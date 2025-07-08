using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using BlogFlow.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/api/posts", async (IDbConnection db) =>
{
    var posts = await db.QueryAsync<Post>("SELECT * FROM Posts ORDER BY CreatedAt DESC");
    return Results.Ok(posts);
});

app.MapGet("/api/posts/{id}", async (int id, IDbConnection db) =>
{
    var post = await db.QueryFirstOrDefaultAsync<Post>("SELECT * FROM Posts WHERE Id = @Id", new { Id = id });
    return post is not null ? Results.Ok(post) : Results.NotFound();
});

app.MapPost("/api/posts", async (Post post, IDbConnection db) =>
{
    post.CreatedAt = DateTime.UtcNow;
    var sql = @"INSERT INTO Posts (Title, Content, CreatedAt) 
                VALUES (@Title, @Content, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";
    var newId = await db.ExecuteScalarAsync<int>(sql, post);
    post.Id = newId;
    return Results.Created($"/api/posts/{post.Id}", post);
});

app.MapPut("/api/posts/{id}", async (int id, Post updated, IDbConnection db) =>
{
    var sql = @"UPDATE Posts SET Title = @Title, Content = @Content WHERE Id = @Id";
    var affected = await db.ExecuteAsync(sql, new { updated.Title, updated.Content, Id = id });
    return affected > 0 ? Results.Ok(updated) : Results.NotFound();
});

app.MapDelete("/api/posts/{id}", async (int id, IDbConnection db) =>
{
    var sql = "DELETE FROM Posts WHERE Id = @Id";
    var affected = await db.ExecuteAsync(sql, new { Id = id });
    return affected > 0 ? Results.NoContent() : Results.NotFound();
});

app.Run();
