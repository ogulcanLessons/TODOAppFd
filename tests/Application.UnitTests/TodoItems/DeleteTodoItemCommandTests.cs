using MediatR;
using Moq;
using Shouldly;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.TodoItems.Commands.DeleteTodoItem;
using Todo_App.Domain.Entities;
using Xunit;

namespace Todo_App.Application.UnitTests.TodoItems;
public class DeleteTodoItemCommandTests
{
    [Fact]
    public async Task Handle_ValidId_ShouldDeleteTodoItem()
    {
        var todoItem = new TodoItem { Id = 1, IsDeleted = false };
        var dbContextMock = new Mock<IApplicationDbContext>();
        dbContextMock.Setup(c => c.TodoItems.FindAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(todoItem);

        var handler = new DeleteTodoItemCommandHandler(dbContextMock.Object);

        // Act
        var result = await handler.Handle(new DeleteTodoItemCommand(1), CancellationToken.None);

        // Assert
        result.ShouldBe(Unit.Value);
        todoItem.IsDeleted.ShouldBe(true);
        dbContextMock.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidId_ShouldThrowNotFoundException()
    {
        var dbContextMock = new Mock<IApplicationDbContext>();
        dbContextMock.Setup(c => c.TodoItems.FindAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem)null);

        var handler = new DeleteTodoItemCommandHandler(dbContextMock.Object);

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(() => handler.Handle(new DeleteTodoItemCommand(1), CancellationToken.None));
        dbContextMock.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Never);
    }
}
