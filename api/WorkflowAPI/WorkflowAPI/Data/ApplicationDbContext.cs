using Microsoft.EntityFrameworkCore;
using WorkflowAPI.Models;

namespace WorkflowAPI.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<StateType> StateTypes { get; set; }
        public DbSet<Transition> Transitions { get; set; }
        public DbSet<TransitionAction> TransitionActions { get; set; }
        public DbSet<RequestNote> RequestNotes { get; set; }
        public DbSet<RequestData> RequestData { get; set; }
        public DbSet<RequestFile> RequestFiles { get; set; }
        public DbSet<RequestStakeholder> RequestStakeholders { get; set; }
        public DbSet<RequestAction> RequestActions { get; set; }
        public DbSet<WorkflowAction> WorkflowActions { get; set; }
        public DbSet<ActionType> ActionTypes { get; set; }
        public DbSet<ActionTarget> ActionTargets { get; set; }
        public DbSet<Target> Targets { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<ActivityTarget> ActivityTargets { get; set; }
        public DbSet<TransitionActivity> TransitionActivities { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<CustomEntity> CustomEntities { get; set; }
        public DbSet<ProcessAdmin> ProcessAdmins { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // GroupMember: composite key
            modelBuilder.Entity<GroupMember>()
                .HasKey(gm => new { gm.UserID, gm.GroupID });
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMembers)
                .HasForeignKey(gm => gm.UserID);
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.GroupMembers)
                .HasForeignKey(gm => gm.GroupID);

            // State - Transition
            modelBuilder.Entity<Transition>()
                .HasOne(t => t.CurrentState)
                .WithMany(s => s.TransitionsFrom)
                .HasForeignKey(t => t.CurrentStateID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Transition>()
                .HasOne(t => t.NextState)
                .WithMany(s => s.TransitionsTo)
                .HasForeignKey(t => t.NextStateID)
                .OnDelete(DeleteBehavior.Restrict);

            // RequestAction - WorkflowAction
            modelBuilder.Entity<RequestAction>()
                .HasOne(ra => ra.Transition)
                .WithMany(t => t.RequestActions)
                .HasForeignKey(ra => ra.TransitionID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RequestAction>()
                .HasOne(ra => ra.Action)
                .WithMany(a => a.RequestActions)
                .HasForeignKey(ra => ra.ActionID);

            // ActionTarget
            modelBuilder.Entity<ActionTarget>()
                .HasKey(at => new { at.ActionID, at.TargetID, at.GroupID });
            modelBuilder.Entity<ActionTarget>()
                .HasOne(at => at.Action)
                .WithMany(a => a.ActionTargets)
                .HasForeignKey(at => at.ActionID);
            modelBuilder.Entity<ActionTarget>()
                .HasOne(at => at.Target)
                .WithMany(t => t.ActionTargets)
                .HasForeignKey(at => at.TargetID);
            modelBuilder.Entity<ActionTarget>()
                .HasOne(at => at.Group)
                .WithMany(g => g.ActionTargets)
                .HasForeignKey(at => at.GroupID);

            // ActivityTarget
            modelBuilder.Entity<ActivityTarget>()
                .HasKey(at => new { at.ActivityID, at.TargetID, at.GroupID });
            modelBuilder.Entity<ActivityTarget>()
                .HasOne(at => at.Activity)
                .WithMany(a => a.ActivityTargets)
                .HasForeignKey(at => at.ActivityID);
            modelBuilder.Entity<ActivityTarget>()
                .HasOne(at => at.Target)
                .WithMany(t => t.ActivityTargets)
                .HasForeignKey(at => at.TargetID);
            modelBuilder.Entity<ActivityTarget>()
                .HasOne(at => at.Group)
                .WithMany(g => g.ActivityTargets)
                .HasForeignKey(at => at.GroupID);

            // TransitionActivity
            modelBuilder.Entity<TransitionActivity>()
                .HasKey(ta => new { ta.TransitionID, ta.ActivityID });
            modelBuilder.Entity<TransitionActivity>()
                .HasOne(ta => ta.Transition)
                .WithMany(t => t.TransitionActivities)
                .HasForeignKey(ta => ta.TransitionID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransitionActivity>()
                .HasOne(ta => ta.Activity)
                .WithMany(a => a.TransitionActivities)
                .HasForeignKey(ta => ta.ActivityID)
                .OnDelete(DeleteBehavior.Restrict);


            // ProcessAdmin: composite key
            modelBuilder.Entity<ProcessAdmin>()
                .HasKey(pa => new { pa.ProcessID, pa.UserID });
            modelBuilder.Entity<ProcessAdmin>()
                .HasOne(pa => pa.Process)
                .WithMany(p => p.Admins)
                .HasForeignKey(pa => pa.ProcessID);
            modelBuilder.Entity<ProcessAdmin>()
                .HasOne(pa => pa.User)
                .WithMany(u => u.ProcessAdmins)
                .HasForeignKey(pa => pa.UserID);

            // TransitionAction: composite key
            modelBuilder.Entity<TransitionAction>()
                .HasKey(ta => new { ta.TransitionID, ta.ActionID });
            modelBuilder.Entity<TransitionAction>()
                .HasOne(ta => ta.Transition)
                .WithMany(t => t.TransitionActions)
                .HasForeignKey(ta => ta.TransitionID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransitionAction>()
                .HasOne(ta => ta.Action)
                .WithMany(a => a.TransitionActions)
                .HasForeignKey(ta => ta.ActionID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .HasOne(st => st.CurrentState)
                .WithMany(r => r.Requests)
                .HasForeignKey(st => st.CurrentStateID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RequestFile>()
                .HasOne(st => st.User)
                .WithMany(r => r.RequestFiles)
                .HasForeignKey(sr => sr.UserID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RequestNote>()
                .HasOne(rn => rn.User)
                .WithMany(u => u.RequestNotes)
                .HasForeignKey(rn => rn.UserID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RequestStakeholder>()
                            .HasKey(pa => new { pa.RequestID, pa.UserID });
            modelBuilder.Entity<RequestStakeholder>()
                .HasOne(rs => rs.User)
                .WithMany(u => u.RequestStakeholders)
                .HasForeignKey(rs => rs.UserID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
