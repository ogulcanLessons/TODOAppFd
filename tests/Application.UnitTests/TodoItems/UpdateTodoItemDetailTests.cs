using MediatR;
using Moq;
using Shouldly;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.TodoItems.Commands.UpdateTodoItemDetail;
using Todo_App.Domain.Entities;
using Todo_App.Domain.Enums;
using Xunit;

namespace Todo_App.Application.UnitTests.TodoItems;
public class UpdateTodoItemDetailTests
{
    [Fact]
    public async Task Handle_ValidId_ShouldUpdateTodoItemDetail()
    {
        var todoItem = new TodoItem { Id = 1 };
        var dbContextMock = new Mock<IApplicationDbContext>();
        dbContextMock.Setup(c => c.TodoItems.FindAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(todoItem);

        var handler = new UpdateTodoItemDetailCommandHandler(dbContextMock.Object);

        var request = new UpdateTodoItemDetailCommand
        {
            Id = 1,
            ListId = 2,
            Priority = PriorityLevel.High,
            Note = "New Note",
            Color = "#FF0000"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBe(Unit.Value);
        todoItem.ListId.ShouldBe(request.ListId);
        todoItem.Priority.ShouldBe(request.Priority);
        todoItem.Note.ShouldBe(request.Note);
        todoItem.Color.ShouldBe(request.Color);
        dbContextMock.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidId_ShouldThrowNotFoundException()
    {
        var dbContextMock = new Mock<IApplicationDbContext>();
        dbContextMock.Setup(c => c.TodoItems.FindAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem)null);

        var handler = new UpdateTodoItemDetailCommandHandler(dbContextMock.Object);

        var request = new UpdateTodoItemDetailCommand
        {
            Id = 1,
            ListId = 2,
            Priority = PriorityLevel.High,
            Note = "New Note",
            Color = "#FF0000"
        };

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(() => handler.Handle(request, CancellationToken.None));
        dbContextMock.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Never);
    }
}
