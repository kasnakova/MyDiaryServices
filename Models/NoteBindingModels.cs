namespace MyDiary.Services.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using MyDiary.Models;

    public class SaveNoteBindingModel
    {
        [Required]
        [Display(Name = "NoteText")]
        public string NoteText { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [MinLength(6)]
        public string Password { get; set; }

        //[Required]
        [Display(Name = "NoteType")]
        public NoteType NoteType { get; set; }
    }
}