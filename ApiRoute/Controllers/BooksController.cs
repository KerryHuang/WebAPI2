﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ApiRoute.Models;

namespace ApiRoute.Controllers
{
    [RoutePrefix("books")]
    public class BooksController : ApiController
    {
        private ApiRouteContext db = new ApiRouteContext();

        [Route("")]
        // GET: api/Books
        public IQueryable<Book> GetBooks()
        {
            return db.Books;
        }

        // GET: api/Books/5
        [Route("{id:int}")]                
        [ResponseType(typeof(Book))]
        public IHttpActionResult GetBook(int id)
        {
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [Route("{genre}")]
        public IHttpActionResult GetBookByGenre(string genre)
        {
            var books = db.Books.Include(b => b.Author)
                .Where(b => b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));

            return Ok(books);
        }

        [Route("date/{*pubdate:datetime}")]
        public IHttpActionResult Get(DateTime pubdate)
        {
            var books = db.Books.Include(b => b.Author)
                .Where(b => DbFunctions.TruncateTime(b.PublishDate)
                         == DbFunctions.TruncateTime(pubdate));

            return Ok(books);
        }

        // PUT: api/Books/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBook(int id, Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.BookId)
            {
                return BadRequest();
            }

            db.Entry(book).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Books
        [ResponseType(typeof(Book))]
        public IHttpActionResult PostBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Books.Add(book);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = book.BookId }, book);
        }

        // DELETE: api/Books/5
        [ResponseType(typeof(Book))]
        public IHttpActionResult DeleteBook(int id)
        {
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            db.Books.Remove(book);
            db.SaveChanges();

            return Ok(book);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.BookId == id) > 0;
        }
    }
}