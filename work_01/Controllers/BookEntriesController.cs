using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using work_01;
using work_01.DTO;

namespace work_01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookEntriesController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookEntriesController(BookingDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/BookEntries
        [HttpGet]
        [Route("GetBooks")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }
        [HttpGet]
        [Route("GetCustomers")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }
        // GET: api/BookEntries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBookEntries()
        {
            List<BookDTO> buyingBooks = new List<BookDTO>();

            var allCustomers = _context.Customers.ToList();
            foreach (var customer in allCustomers)
            {
                var bookList = _context.BookEntries.Where(x => x.CustomerId == customer.CustomerId).Select(x => new Book { BookId = x.BookId }).ToList();
                buyingBooks.Add(new BookDTO
                {
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.CustomerName,
                    DateOfBirth = customer.DateOfBirth,
                    Phone = customer.Phone,
                    MaritialStatus = customer.MaritialStatus,
                    Picture = customer.Picture,
                    BookItems = bookList.ToArray()
                });
            }


            return buyingBooks;
        }
        // GET: api/BookEntries
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBookEntries(int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);
            Book[] bookList = _context.BookEntries.Where(x => x.CustomerId == customer.CustomerId).Select(x => new Book { BookId = x.BookId }).ToArray();

            BookDTO buyingBook = new BookDTO()
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                DateOfBirth = customer.DateOfBirth,
                Phone = customer.Phone,
                MaritialStatus = customer.MaritialStatus,
                Picture = customer.Picture,
                BookItems = bookList
            };

            return buyingBook;
        }

        // POST: api/BookEntries
        [HttpPost]
        public async Task<ActionResult<BookEntry>> PostBookingEntry([FromForm] BookDTO bookDTO)
        {

            var bookItems = JsonConvert.DeserializeObject<Book[]>(bookDTO.booksStringify);

            Customer customer = new Customer
            {
                CustomerName = bookDTO.CustomerName,
                DateOfBirth = bookDTO.DateOfBirth,
                Phone = bookDTO.Phone,
                MaritialStatus = bookDTO.MaritialStatus
            };

            if (bookDTO.PictureFile != null)
            {
                var webroot = _env.WebRootPath;
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bookDTO.PictureFile.FileName);
                var filePath = Path.Combine(webroot, "Images", fileName);

                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                await bookDTO.PictureFile.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                fileStream.Close();
                customer.Picture = fileName;
            }

            foreach (var item in bookItems)
            {
                var bookEntry = new BookEntry
                {
                    Customer = customer,
                    CustomerId = customer.CustomerId,
                    BookId = item.BookId
                };
                _context.Add(bookEntry);
            }

            await _context.SaveChangesAsync();

            return Ok(customer);
        }

        //Update BookiBuying
        // POST: api/BookEntries/Update
        [Route("Update")]
        [HttpPost]
        public async Task<ActionResult<BookEntry>> UpdateBookEntry([FromForm] BookDTO bookDTO)
        {

            var bookItems = JsonConvert.DeserializeObject<Book[]>(bookDTO.booksStringify);

            Customer customer = _context.Customers.Find(bookDTO.CustomerId);
            customer.CustomerName = bookDTO.CustomerName;
            customer.DateOfBirth = bookDTO.DateOfBirth;
            customer.Phone = bookDTO.Phone;
            customer.MaritialStatus = bookDTO.MaritialStatus;


            if (bookDTO.PictureFile != null)
            {
                var webroot = _env.WebRootPath;
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bookDTO.PictureFile.FileName);
                var filePath = Path.Combine(webroot, "Images", fileName);

                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                await bookDTO.PictureFile.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                fileStream.Close();
                customer.Picture = fileName;
            }

            //Delete existing spots
            var existingBooks = _context.BookEntries.Where(x => x.CustomerId == customer.CustomerId).ToList();
            foreach (var item in existingBooks)
            {
                _context.BookEntries.Remove(item);
            }

            //Add newly added spots
            foreach (var item in bookItems)
            {
                var bookEntry = new BookEntry
                {
                    CustomerId = customer.CustomerId,
                    BookId = item.BookId
                };
                _context.Add(bookEntry);
            }

            _context.Entry(customer).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(customer);
        }

        //Delete Book
        // POST: api/BookEntries/Update
        [Route("Delete/{id}")]
        [HttpPost]
        public async Task<ActionResult<BookEntry>> DeleteBookEntry(int id)
        {

            Customer customer = _context.Customers.Find(id);

            var existingBooks = _context.BookEntries.Where(x => x.CustomerId == customer.CustomerId).ToList();
            foreach (var item in existingBooks)
            {
                _context.BookEntries.Remove(item);
            }

            _context.Entry(customer).State = EntityState.Deleted;

            await _context.SaveChangesAsync();

            return Ok(customer);
        }
    }
}
