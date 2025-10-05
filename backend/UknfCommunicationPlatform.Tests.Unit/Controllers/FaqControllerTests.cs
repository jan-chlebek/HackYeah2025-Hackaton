using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UknfCommunicationPlatform.Api.Controllers;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Controllers;

public class FaqControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly FaqController _controller;
    private readonly Mock<ILogger<FaqController>> _loggerMock;

    public FaqControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<FaqController>>();
        _controller = new FaqController(_context, _loggerMock.Object);

        SeedTestData();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private void SeedTestData()
    {
        var faqs = new List<FaqQuestion>
        {
            new FaqQuestion
            {
                Id = 1,
                Question = "Jak mogę zalogować się do systemu?",
                Answer = "<p>Aby zalogować się do systemu, należy wprowadzić login i hasło.</p>"
            },
            new FaqQuestion
            {
                Id = 2,
                Question = "Jak często należy składać raporty kwartalne?",
                Answer = "<p>Raporty kwartalne należy składać cztery razy w roku.</p>"
            },
            new FaqQuestion
            {
                Id = 3,
                Question = "Jakie formaty plików są akceptowane?",
                Answer = "<p>System akceptuje wyłącznie pliki w formacie XLSX.</p>"
            },
            new FaqQuestion
            {
                Id = 4,
                Question = "Gdzie mogę znaleźć archiwalne dokumenty?",
                Answer = "<p>Archiwalne dokumenty znajdują się w zakładce Biblioteka plików.</p>"
            },
            new FaqQuestion
            {
                Id = 5,
                Question = "Co zrobić w przypadku problemów technicznych?",
                Answer = "<p>W przypadku problemów technicznych należy skontaktować się z helpdesk.</p>"
            }
        };

        _context.FaqQuestions.AddRange(faqs);
        _context.SaveChanges();
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsAllFaqs_WhenNoSearchProvided()
    {
        // Act
        var result = await _controller.GetAll(null, 1, 20);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var pagedResult = okResult.Value.Should().BeAssignableTo<PagedResult<FaqQuestionDto>>().Subject;
        
        pagedResult.Items.Should().HaveCount(5);
        pagedResult.TotalCount.Should().Be(5);
        pagedResult.Page.Should().Be(1);
        pagedResult.PageSize.Should().Be(20);
        pagedResult.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task GetAll_ReturnsPaginatedResults_WhenPageSizeIsSmall()
    {
        // Act
        var result = await _controller.GetAll(null, 2, 2);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var pagedResult = okResult.Value.Should().BeAssignableTo<PagedResult<FaqQuestionDto>>().Subject;
        
        pagedResult.Items.Should().HaveCount(2);
        pagedResult.TotalCount.Should().Be(5);
        pagedResult.Page.Should().Be(2);
        pagedResult.PageSize.Should().Be(2);
        pagedResult.TotalPages.Should().Be(3);
        pagedResult.Items.First().Id.Should().Be(3);
    }

    [Fact]
    public async Task GetAll_ReturnsFilteredResults_WhenSearchProvided()
    {
        // Act
        var result = await _controller.GetAll("raport", 1, 20);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var pagedResult = okResult.Value.Should().BeAssignableTo<PagedResult<FaqQuestionDto>>().Subject;
        
        pagedResult.Items.Should().HaveCount(1);
        pagedResult.TotalCount.Should().Be(1);
        pagedResult.Items.First().Question.Should().Contain("raport");
    }

    [Fact]
    public async Task GetAll_SearchIsCaseInsensitive()
    {
        // Act
        var result = await _controller.GetAll("RAPORT", 1, 20);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var pagedResult = okResult.Value.Should().BeAssignableTo<PagedResult<FaqQuestionDto>>().Subject;
        
        pagedResult.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAll_SearchesInBothQuestionAndAnswer()
    {
        // Act
        var result = await _controller.GetAll("XLSX", 1, 20);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var pagedResult = okResult.Value.Should().BeAssignableTo<PagedResult<FaqQuestionDto>>().Subject;
        
        pagedResult.Items.Should().HaveCount(1);
        pagedResult.Items.First().Id.Should().Be(3);
    }

    [Fact]
    public async Task GetAll_DefaultsToPage1_WhenPageIsLessThan1()
    {
        // Act
        var result = await _controller.GetAll(null, 0, 20);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var pagedResult = okResult.Value.Should().BeAssignableTo<PagedResult<FaqQuestionDto>>().Subject;
        
        pagedResult.Page.Should().Be(1);
    }

    [Fact]
    public async Task GetAll_LimitsPageSizeTo100_WhenPageSizeExceeds100()
    {
        // Act
        var result = await _controller.GetAll(null, 1, 200);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var pagedResult = okResult.Value.Should().BeAssignableTo<PagedResult<FaqQuestionDto>>().Subject;
        
        pagedResult.PageSize.Should().Be(20); // Default when invalid
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoMatchFound()
    {
        // Act
        var result = await _controller.GetAll("nonexistent search term xyz123", 1, 20);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var pagedResult = okResult.Value.Should().BeAssignableTo<PagedResult<FaqQuestionDto>>().Subject;
        
        pagedResult.Items.Should().BeEmpty();
        pagedResult.TotalCount.Should().Be(0);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ReturnsFaq_WhenFaqExists()
    {
        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var faqDto = okResult.Value.Should().BeAssignableTo<FaqQuestionDto>().Subject;
        
        faqDto.Id.Should().Be(1);
        faqDto.Question.Should().Be("Jak mogę zalogować się do systemu?");
        faqDto.Answer.Should().Contain("login i hasło");
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenFaqDoesNotExist()
    {
        // Act
        var result = await _controller.GetById(999);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().NotBeNull();
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_AddsFaqToDatabase_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateFaqQuestionRequest
        {
            Question = "Nowe pytanie testowe?",
            Answer = "<p>To jest testowa odpowiedź.</p>"
        };

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var faqDto = createdResult.Value.Should().BeAssignableTo<FaqQuestionDto>().Subject;
        
        faqDto.Question.Should().Be("Nowe pytanie testowe?");
        faqDto.Answer.Should().Be("<p>To jest testowa odpowiedź.</p>");

        var dbFaq = await _context.FaqQuestions.FindAsync(faqDto.Id);
        dbFaq.Should().NotBeNull();
        dbFaq!.Question.Should().Be("Nowe pytanie testowe?");
    }

    [Fact]
    public async Task Create_TrimsWhitespace_FromQuestionAndAnswer()
    {
        // Arrange
        var request = new CreateFaqQuestionRequest
        {
            Question = "  Pytanie z spacjami  ",
            Answer = "  Odpowiedź z spacjami  "
        };

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var faqDto = createdResult.Value.Should().BeAssignableTo<FaqQuestionDto>().Subject;
        
        faqDto.Question.Should().Be("Pytanie z spacjami");
        faqDto.Answer.Should().Be("Odpowiedź z spacjami");
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithCorrectRoute()
    {
        // Arrange
        var request = new CreateFaqQuestionRequest
        {
            Question = "Test?",
            Answer = "Test answer"
        };

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(FaqController.GetById));
        createdResult.RouteValues.Should().ContainKey("id");
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_UpdatesFaqInDatabase_WhenFaqExists()
    {
        // Arrange
        var request = new UpdateFaqQuestionRequest
        {
            Question = "Zaktualizowane pytanie?",
            Answer = "<p>Zaktualizowana odpowiedź.</p>"
        };

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var faqDto = okResult.Value.Should().BeAssignableTo<FaqQuestionDto>().Subject;
        
        faqDto.Id.Should().Be(1);
        faqDto.Question.Should().Be("Zaktualizowane pytanie?");
        faqDto.Answer.Should().Be("<p>Zaktualizowana odpowiedź.</p>");

        var dbFaq = await _context.FaqQuestions.FindAsync(1L);
        dbFaq!.Question.Should().Be("Zaktualizowane pytanie?");
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenFaqDoesNotExist()
    {
        // Arrange
        var request = new UpdateFaqQuestionRequest
        {
            Question = "Test?",
            Answer = "Test answer"
        };

        // Act
        var result = await _controller.Update(999, request);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_TrimsWhitespace_FromQuestionAndAnswer()
    {
        // Arrange
        var request = new UpdateFaqQuestionRequest
        {
            Question = "  Updated with spaces  ",
            Answer = "  Updated answer with spaces  "
        };

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var faqDto = okResult.Value.Should().BeAssignableTo<FaqQuestionDto>().Subject;
        
        faqDto.Question.Should().Be("Updated with spaces");
        faqDto.Answer.Should().Be("Updated answer with spaces");
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_RemovesFaqFromDatabase_WhenFaqExists()
    {
        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        var dbFaq = await _context.FaqQuestions.FindAsync(1L);
        dbFaq.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenFaqDoesNotExist()
    {
        // Act
        var result = await _controller.Delete(999);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_DoesNotAffectOtherFaqs()
    {
        // Arrange
        var initialCount = await _context.FaqQuestions.CountAsync();

        // Act
        await _controller.Delete(1);

        // Assert
        var remainingCount = await _context.FaqQuestions.CountAsync();
        remainingCount.Should().Be(initialCount - 1);

        var otherFaq = await _context.FaqQuestions.FindAsync(2L);
        otherFaq.Should().NotBeNull();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task FullCrudWorkflow_WorksCorrectly()
    {
        // Create
        var createRequest = new CreateFaqQuestionRequest
        {
            Question = "Integration test question?",
            Answer = "<p>Integration test answer.</p>"
        };
        var createResult = await _controller.Create(createRequest);
        var createdFaq = ((CreatedAtActionResult)createResult).Value as FaqQuestionDto;

        // Read
        var readResult = await _controller.GetById(createdFaq!.Id);
        var readFaq = ((OkObjectResult)readResult).Value as FaqQuestionDto;
        readFaq!.Question.Should().Be("Integration test question?");

        // Update
        var updateRequest = new UpdateFaqQuestionRequest
        {
            Question = "Updated integration test question?",
            Answer = "<p>Updated integration test answer.</p>"
        };
        var updateResult = await _controller.Update(createdFaq.Id, updateRequest);
        var updatedFaq = ((OkObjectResult)updateResult).Value as FaqQuestionDto;
        updatedFaq!.Question.Should().Be("Updated integration test question?");

        // Delete
        var deleteResult = await _controller.Delete(createdFaq.Id);
        deleteResult.Should().BeOfType<NoContentResult>();

        // Verify deletion
        var verifyResult = await _controller.GetById(createdFaq.Id);
        verifyResult.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetAll_ReturnsCorrectTotalPages_WithDifferentPageSizes()
    {
        // Test with page size 2 (should have 3 pages for 5 items)
        var result1 = await _controller.GetAll(null, 1, 2);
        var pagedResult1 = ((OkObjectResult)result1).Value as PagedResult<FaqQuestionDto>;
        pagedResult1!.TotalPages.Should().Be(3);

        // Test with page size 3 (should have 2 pages for 5 items)
        var result2 = await _controller.GetAll(null, 1, 3);
        var pagedResult2 = ((OkObjectResult)result2).Value as PagedResult<FaqQuestionDto>;
        pagedResult2!.TotalPages.Should().Be(2);

        // Test with page size 5 (should have 1 page for 5 items)
        var result3 = await _controller.GetAll(null, 1, 5);
        var pagedResult3 = ((OkObjectResult)result3).Value as PagedResult<FaqQuestionDto>;
        pagedResult3!.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Search_FindsFaqsByPartialMatch()
    {
        // Search for "system" should find both the login question and file format answer
        var result = await _controller.GetAll("system", 1, 20);
        var pagedResult = ((OkObjectResult)result).Value as PagedResult<FaqQuestionDto>;
        
        pagedResult!.Items.Should().HaveCount(2);
        pagedResult.Items.Should().Contain(f => f.Question.ToLower().Contains("system"));
        pagedResult.Items.Should().Contain(f => f.Answer.ToLower().Contains("system"));
    }

    #endregion

    #region SubmitQuestion Tests

    [Fact]
    public async Task SubmitQuestion_WithValidQuestion_ReturnsCreatedResult()
    {
        // Arrange
        var request = new SubmitQuestionRequest
        {
            Question = "Jak zmienić adres email?"
        };

        // Act
        var result = await _controller.SubmitQuestion(request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.Value.Should().BeOfType<FaqQuestionDto>();
        
        var dto = createdResult.Value as FaqQuestionDto;
        dto!.Question.Should().Be("Jak zmienić adres email?");
        dto.Answer.Should().BeEmpty();
        dto.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SubmitQuestion_AddsQuestionToDatabase()
    {
        // Arrange
        var request = new SubmitQuestionRequest
        {
            Question = "Jakie dokumenty są wymagane?"
        };

        // Act
        await _controller.SubmitQuestion(request);

        // Assert
        var faq = await _context.FaqQuestions
            .FirstOrDefaultAsync(f => f.Question == "Jakie dokumenty są wymagane?");
        faq.Should().NotBeNull();
        faq!.Answer.Should().BeEmpty();
    }

    [Fact]
    public async Task SubmitQuestion_TrimsWhitespace()
    {
        // Arrange
        var request = new SubmitQuestionRequest
        {
            Question = "  Pytanie z białymi znakami  "
        };

        // Act
        await _controller.SubmitQuestion(request);

        // Assert
        var faq = await _context.FaqQuestions
            .FirstOrDefaultAsync(f => f.Question == "Pytanie z białymi znakami");
        faq.Should().NotBeNull();
    }

    [Fact]
    public async Task SubmitQuestion_WithEmptyQuestion_ReturnsBadRequest()
    {
        // Arrange
        var request = new SubmitQuestionRequest
        {
            Question = ""
        };

        // Act
        var result = await _controller.SubmitQuestion(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.Should().BeOfType<ProblemDetails>();
    }

    [Fact]
    public async Task SubmitQuestion_WithWhitespaceOnly_ReturnsBadRequest()
    {
        // Arrange
        var request = new SubmitQuestionRequest
        {
            Question = "   "
        };

        // Act
        var result = await _controller.SubmitQuestion(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task SubmitQuestion_ReturnsCreatedAtActionWithCorrectRoute()
    {
        // Arrange
        var request = new SubmitQuestionRequest
        {
            Question = "Test question"
        };

        // Act
        var result = await _controller.SubmitQuestion(request);

        // Assert
        var createdResult = result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(FaqController.GetById));
        createdResult.RouteValues.Should().ContainKey("id");
    }

    #endregion
}
