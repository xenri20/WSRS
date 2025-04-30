using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WSRS_SWAFO.Models;

public class HearingSchedules
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public DateTime ScheduledDate { get; set; }

    [Required]
    public int StudentNumber { get; set; }

    [ForeignKey("StudentNumber")]
    public Student Student { get; set; }
}
