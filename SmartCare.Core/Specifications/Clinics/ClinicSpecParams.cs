using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Core.Specifications.Clinics
{
    public class ClinicSpecParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 12;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
