using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Data
{
    public class TodoAppDbContext : DbContext
    {
        public TodoAppDbContext (DbContextOptions<TodoAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Log> Logs { get; set; } = default!;
        public DbSet<MainTask> MainTasks { get; set; } = default!;
        public DbSet<SubTask> SubTasks { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;

    }
}
