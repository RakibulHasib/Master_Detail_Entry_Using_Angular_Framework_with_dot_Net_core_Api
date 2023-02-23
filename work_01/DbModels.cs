using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace work_01
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Phone { get; set; }
        public string Picture { get; set; }
        public bool MaritialStatus { get; set; }
        public virtual ICollection<BookEntry> BookEntries { get; set; } = new List<BookEntry>();
    }
    public class Book
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public virtual ICollection<BookEntry> BookEntries { get; set; }=new List<BookEntry>();
    }
    public class BookEntry
    {
        public int BookEntryId { get; set; }
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }

        //Nav
        public virtual Customer Customer { get; set; }
        public virtual Book Book { get; set; }
    }
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext>options):base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; } = default!;
        public DbSet<Book> Books { get; set; } = default!;
        public DbSet<BookEntry> BookEntries { get; set; } = default!;
    }
}
