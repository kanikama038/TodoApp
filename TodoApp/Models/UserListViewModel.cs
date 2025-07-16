using TodoApp.Models;

public class UserListViewModel
{
    public List<User> Users { get; set; } = new List<User>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
