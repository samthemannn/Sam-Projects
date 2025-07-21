// Program 1: Abstraction with YouTube Videos

using System;
using System.Collections.Generic;

public class Video
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Length { get; set; } // in seconds
    private List<Comment> comments = new List<Comment>();

    public void AddComment(Comment comment)
    {
        comments.Add(comment);
    }

    public int GetNumberOfComments()
    {
        return comments.Count;
    }

    public List<Comment> GetComments()
    {
        return comments;
    }
}

public class Comment
{
    public string CommenterName { get; set; }
    public string Text { get; set; }
}

public class Abstraction
{
    public static void Run()
    {
        List<Video> videos = new List<Video>();

        Video video1 = new Video
        {
            Title = "C# Tutorial for Beginners",
            Author = "Tech Tutorials",
            Length = 600
        };
        video1.AddComment(new Comment { CommenterName = "Alice", Text = "Great tutorial!" });
        video1.AddComment(new Comment { CommenterName = "Bob", Text = "Very helpful, thanks!" });
        video1.AddComment(new Comment { CommenterName = "Charlie", Text = "I learned a lot." });
        videos.Add(video1);

        Video video2 = new Video
        {
            Title = "Funny Cat Compilation",
            Author = "CatLover123",
            Length = 300
        };
        video2.AddComment(new Comment { CommenterName = "David", Text = "So funny! I love cats." });
        video2.AddComment(new Comment { CommenterName = "Eve", Text = "My cat does the same thing!" });
        video2.AddComment(new Comment { CommenterName = "Frank", Text = "LOL" });
        videos.Add(video2);

        Video video3 = new Video
        {
            Title = "How to Cook Pasta",
            Author = "Chef John",
            Length = 450
        };
        video3.AddComment(new Comment { CommenterName = "Grace", Text = "Delicious recipe!" });
        video3.AddComment(new Comment { CommenterName = "Heidi", Text = "I'm making this tonight." });
        video3.AddComment(new Comment { CommenterName = "Ivan", Text = "Thanks for sharing." });
        videos.Add(video3);

        foreach (var video in videos)
        {
            Console.WriteLine($"Title: {video.Title}");
            Console.WriteLine($"Author: {video.Author}");
            Console.WriteLine($"Length: {video.Length} seconds");
            Console.WriteLine($"Number of comments: {video.GetNumberOfComments()}");
            Console.WriteLine("Comments:");
            foreach (var comment in video.GetComments())
            {
                Console.WriteLine($"- {comment.CommenterName}: {comment.Text}");
            }
            Console.WriteLine();
        }
    }
}
