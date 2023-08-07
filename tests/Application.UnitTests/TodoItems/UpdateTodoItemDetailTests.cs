using Moq;
using Todo_App.Application.TodoItems.Commands.UpdateTodoItemDetail;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;
using Xunit;
using Todo_App.Domain.Enums;
using NUnit.Framework;

namespace Todo_App.Application.UnitTests.TodoItems;
public class UpdateTodoItemDetailTests
{
    [Fact]
    public async Task Handle_ValidId_ShouldUpdateTodoItemDetail()
    {
        // Arrange
        var dbContextMock = new Mock<IApplicationDbContext>();
        var todoItemId = 1;
        var updatedListId = 2;
        var updatedPriority = PriorityLevel.High;
        var updatedNote = "Updated note";
        var updatedTags = "tag1,tag2,tag3";

        var existingTodoItem = new TodoItem
        {
            Id = todoItemId,
            ListId = 1,
            Priority = PriorityLevel.Low,
            Note = "Initial note",
            Tags = "initial,tag"
        };

        dbContextMock.Setup(x => x.TodoItems.FindAsync(new object[] { todoItemId }, CancellationToken.None))
            .ReturnsAsync(existingTodoItem);

        var handler = new UpdateTodoItemDetailCommandHandler(dbContextMock.Object);

        var command = new UpdateTodoItemDetailCommand
        {
            Id = todoItemId,
            ListId = updatedListId,
            Priority = updatedPriority,
            Note = updatedNote,
            Tags = updatedTags
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        Assert.AreEqual(existingTodoItem.ListId, updatedListId);
        Assert.AreEqual(existingTodoItem.Priority, updatedPriority);
        Assert.AreEqual(existingTodoItem.Note, updatedNote);
        Assert.AreEqual(existingTodoItem.Tags, updatedTags);
    }
}
