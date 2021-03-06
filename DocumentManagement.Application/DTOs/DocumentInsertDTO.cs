﻿using System.ComponentModel.DataAnnotations;

namespace DocumentManagement.Application.DTOs
{
    public class DocumentInsertDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public long Size { get; set; }
        [Required]
        public string Format { get; set; }
        [Required]
        public string ContentBase64 { get; set; }
    }
}
