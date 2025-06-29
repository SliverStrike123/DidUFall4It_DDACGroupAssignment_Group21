using DidUFall4It_DDACGroupAssignment_Group21.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DidUFall4It_DDACGroupAssignment_Group21.Models;

namespace DidUFall4It_DDACGroupAssignment_Group21.Data;

public class DidUFall4It_DDACGroupAssignment_Group21Context : IdentityDbContext<DidUFall4It_DDACGroupAssignment_Group21User>
{
    public DidUFall4It_DDACGroupAssignment_Group21Context(DbContextOptions<DidUFall4It_DDACGroupAssignment_Group21Context> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
    public DbSet<InfographicModel> Infographics { get; set; }
    public DbSet<QuizModel> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuizReview> QuizReviews { get; set; }
    public DbSet<QuizAttempt> QuizAttempts { get; set; }
    public DbSet<InfographicFeedback> InfographicFeedback { get; set; }

}
