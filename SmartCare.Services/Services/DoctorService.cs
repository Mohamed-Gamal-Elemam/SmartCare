using AutoMapper;
using SmartCare.Core.Entities;
using SmartCare.Core.Entities.Climic;
using SmartCare.Core.Entities.Doctors;
using SmartCare.Core.Entities.Identity;
using SmartCare.Core.Interfaces;
using SmartCare.Core.Specifications.Doctors;
using SmartCare.Services.Abstraction.DTOs.Doctors;
using SmartCare.Services.Abstraction.Exceptions;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Shared.Responses;


namespace SmartCare.Services.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DoctorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<DoctorDto>> GetDoctorsAsync(DoctorSpecParams specParams)
        {
            // For Public / Patients: Force Active Only
            specParams.IsActive = true;
            return await FetchPaginatedDoctorsAsync(specParams);
        }

        public async Task<PaginatedResponse<DoctorDto>> GetAdminDoctorsAsync(DoctorSpecParams specParams)
        {
            // Admin gets all active/inactive based on query params
            return await FetchPaginatedDoctorsAsync(specParams);
        }

      

        public async Task<DoctorDto> GetDoctorByIdAsync(Guid id)
        {
            var spec = new DoctorSpecification(id);
            var doctors = await _unitOfWork.GetRepository<Doctor, Guid>().GetBySpecificationAsync(spec);
            var doctor = doctors.FirstOrDefault();

            if (doctor == null)
                throw new NotFoundException($"Doctor with ID '{id}' was not found.");

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto> GetDoctorByUserIdAsync(Guid userId)
        {
            var spec = new DoctorSpecification(userId);
            var doctors = await _unitOfWork.GetRepository<Doctor, Guid>().GetBySpecificationAsync(spec);
            var doctor = doctors.FirstOrDefault();

            if (doctor == null)
                throw new NotFoundException($"Doctor profile for User ID '{userId}' was not found.");

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, Guid>();

            // 1. Ensure User doesn't already have a Doctor profile
            var userSpec = new DoctorSpecification(createDoctorDto.UserId);
            var existingDoctors = await doctorRepo.GetBySpecificationAsync(userSpec);
            if (existingDoctors.Any())
                throw new ConflictException($"User with ID '{createDoctorDto.UserId}' already has a Doctor profile.");

            // 2. Validate User Existence 
            var user = await _unitOfWork.GetRepository<ApplicationUser, Guid>().GetByIdAsync(createDoctorDto.UserId);
            if (user == null)
                throw new NotFoundException($"User with ID '{createDoctorDto.UserId}' does not exist.");

            // 3. Validate Specialty Existence
            var specialty = await _unitOfWork.GetRepository<Specialty, Guid>().GetByIdAsync(createDoctorDto.SpecialtyId);
            if (specialty == null)
                throw new NotFoundException($"Specialty with ID '{createDoctorDto.SpecialtyId}' does not exist.");

            // 4. Validate Clinic Existence
            var clinic = await _unitOfWork.GetRepository<Clinic, Guid>().GetByIdAsync(createDoctorDto.ClinicId);
            if (clinic == null)
                throw new NotFoundException($"Clinic with ID '{createDoctorDto.ClinicId}' does not exist.");

            // 5. Create Entity
            var doctor = _mapper.Map<Doctor>(createDoctorDto);
            doctor.Id = Guid.NewGuid();
            doctor.CreatedAt = DateTime.UtcNow;
            doctor.IsActive = true;

            await doctorRepo.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            return await GetDoctorByIdAsync(doctor.Id);
        }

        public async Task<DoctorDto> UpdateDoctorAsync(Guid id, UpdateDoctorDto updateDoctorDto)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, Guid>();
            var doctor = await doctorRepo.GetByIdAsync(id);

            if (doctor == null)
                throw new NotFoundException($"Doctor with ID '{id}' was not found.");

            if (doctor.SpecialtyId != updateDoctorDto.SpecialtyId)
            {
                var specialtyExists = await _unitOfWork.GetRepository<Specialty, Guid>().GetByIdAsync(updateDoctorDto.SpecialtyId);
                if (specialtyExists == null)
                    throw new NotFoundException($"Specialty with ID '{updateDoctorDto.SpecialtyId}' does not exist.");
            }

            if (doctor.ClinicId != updateDoctorDto.ClinicId)
            {
                var clinicExists = await _unitOfWork.GetRepository<Clinic, Guid>().GetByIdAsync(updateDoctorDto.ClinicId);
                if (clinicExists == null)
                    throw new NotFoundException($"Clinic with ID '{updateDoctorDto.ClinicId}' does not exist.");
            }

            _mapper.Map(updateDoctorDto, doctor);
            doctor.UpdatedAt = DateTime.UtcNow;

            doctorRepo.Update(doctor);
            await _unitOfWork.SaveChangesAsync();

            return await GetDoctorByIdAsync(doctor.Id);
        }

        public async Task<bool> ActivateDoctorAsync(Guid id)
        {
            return await SetDoctorStatusAsync(id, true);
        }

        public async Task<bool> DeactivateDoctorAsync(Guid id)
        {
            return await SetDoctorStatusAsync(id, false);
        }
        private async Task<PaginatedResponse<DoctorDto>> FetchPaginatedDoctorsAsync(DoctorSpecParams specParams)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, Guid>();

            var pageIndex = specParams.PageIndex < 1 ? 1 : specParams.PageIndex;
            var pageSize = specParams.PageSize < 1 ? DoctorSpecParams.DefaultPageSize : specParams.PageSize;

            var skip = pageSize * (pageIndex - 1);

            var spec = new DoctorSpecification(
                specParams.SpecialtyId,
                specParams.ClinicId,
                specParams.Search,
                specParams.MinFee,
                specParams.MaxFee,
                specParams.IsActive,
                specParams.Sort,
                skip,
                pageSize
            );

            var countSpec = new DoctorCountSpecification(
                specParams.SpecialtyId,
                specParams.ClinicId,
                specParams.Search,
                specParams.MinFee,
                specParams.MaxFee,
                specParams.IsActive
            );

            var doctors = await doctorRepo.GetBySpecificationAsync(spec);
            var totalItems = await doctorRepo.CountAsync(countSpec);

            var mappedData = _mapper.Map<IReadOnlyList<DoctorDto>>(doctors);

            return new PaginatedResponse<DoctorDto>(
                items: mappedData,
                pageNumber: pageIndex,
                pageSize: pageSize,
                totalCount: totalItems
            );
        }

        private async Task<bool> SetDoctorStatusAsync(Guid id, bool status)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, Guid>();
            var spec = new DoctorSpecification(id);
            var doctors = await doctorRepo.GetBySpecificationAsync(spec);
            var doctor = doctors.FirstOrDefault();

            if (doctor == null)
                throw new NotFoundException($"Doctor with ID '{id}' was not found.");

            doctor.IsActive = status;
            doctor.User.UpdatedAt = DateTime.UtcNow;

            doctorRepo.Update(doctor);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
