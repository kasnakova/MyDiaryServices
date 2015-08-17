namespace MyDiary.Services.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;

    using Microsoft.AspNet.Identity;

    using MyDiary.Data;
    using MyDiary.Models;
    using MyDiary.Services.Models;
    using MyDiary.Services.Infrastructure;
    using System.Data.Entity.Core.Objects;

    [Authorize]
    public class NotesController : BaseApiController
    {
        private IUserIdProvider userIdProvider;
        private ISecurityProvider securityProvider;

        public NotesController()
            :this(new MyDiaryData(new MyDiaryDbContext()), new AspNetUserIdProvider())
        {

        }

        public NotesController(IMyDiaryData data,
                                IUserIdProvider userIdProvider)
            : base(data)
        {
            this.userIdProvider = userIdProvider;
            this.securityProvider = new SecurityProvider();
        }

        [HttpPost]
        public IHttpActionResult SaveNote(SaveNoteBindingModel noteModel)
        {
            if (!ModelState.IsValid)
            {
                var dfg = BadRequest(ModelState);
                return BadRequest(ModelState);
            }

            var currentUserId = this.userIdProvider.GetUserId();
            var passwordHash = noteModel.Password;
            var encryptedText = noteModel.NoteText;
            if(noteModel.Password != null)
            {
                passwordHash = this.securityProvider.HashPassword(noteModel.Password);
                encryptedText = this.securityProvider.EncryptText(noteModel.NoteText, passwordHash);
            }

            var note = new Note()
            {
                Date = noteModel.Date,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                NoteText = encryptedText,
                PasswordHash = passwordHash,
                Type = NoteType.Normal,
                UserId = currentUserId
            };

            this.data.Notes.Add(note);
            this.data.SaveChanges();

            return Ok(note.Id);
        }

        [HttpGet]
        public IHttpActionResult GetNotes(DateTime date)
        {
            var currentUserId = this.userIdProvider.GetUserId();
            var notes = this.data.Notes.All().Where(n => (currentUserId == n.UserId) && (EntityFunctions.TruncateTime(n.Date) == date.Date)).Select(NoteViewModel.FromNote);
            return Ok(notes);
        }

        [HttpPut]
        public IHttpActionResult UpdateNote(int id, SaveNoteBindingModel noteModel)
        {
            if (!ModelState.IsValid)
            {
                var dfg = BadRequest(ModelState);
                return BadRequest(ModelState);
            }

            var note = this.data.Notes.Find(id);
            if (note == null) 
            {
                return BadRequest("No such note!");
            }

            note.NoteText = noteModel.NoteText;

            this.data.Notes.Update(note);
            this.data.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        public IHttpActionResult DeleteNote([FromUri] int id)
        {
            var deleted = this.data.Notes.Delete(id);
            this.data.SaveChanges();
            if(deleted != null)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IHttpActionResult GetDatesWithNotes(int month, int year)
        {
            if(month > 12)
            {
                return BadRequest();
            }

            var currentUserId = this.userIdProvider.GetUserId();
            var dates = this.data.Notes.All()
                                       .Where(n => (n.UserId == currentUserId) && (n.Date.Month == month) && (n.Date.Year == year))
                                       .Select(n => n.Date);
                
            return Ok(dates);
        }

        [HttpGet]
        public IHttpActionResult GetDecryptedNoteText([FromUri] int id, string password)
        {
            if(id < 0)
            {
                return BadRequest("Invalid id!");
            }

            if (password.Length < 6)
            {
                return BadRequest("Password must be at least 6 characters long!");
            }

            var note = this.data.Notes.Find(id);
            if(note == null)
            {
                return BadRequest("There is no note with id " + id + "!");
            }

            var hashPassword = this.securityProvider.HashPassword(password);
            if(hashPassword != note.PasswordHash)
            {
                return BadRequest("The password is not correct!");
            }

            var decryptedText = this.securityProvider.DecryptText(note.NoteText, hashPassword);

            return Ok(decryptedText);
        }
    }
}