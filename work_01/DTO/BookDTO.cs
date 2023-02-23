using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System;

namespace work_01.DTO
{
    public class BookDTO
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        public int Phone { get; set; }

        public string Picture { get; set; }

        public IFormFile PictureFile { get; set; }

        public bool MaritialStatus { get; set; }

        public string booksStringify { get; set; }

        public Book[] BookItems { get; set; }
    }
}
