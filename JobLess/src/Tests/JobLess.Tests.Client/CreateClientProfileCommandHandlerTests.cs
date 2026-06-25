using ClientEntity = JobLess.Client.Domain.Entities.Client;
using FluentAssertions;
using JobLess.Client.Application.Clients.Commands.CreateClientProfile;
using JobLess.Client.Application.Interfaces;
using JobLess.Client.Domain.Enums;
using MockQueryable.Moq;
using Moq;

namespace JobLess.Tests.Client;

public class CreateClientProfileCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly CreateClientProfileCommandHandler _handler;

    public CreateClientProfileCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new CreateClientProfileCommandHandler(_contextMock.Object);
    }

    private static CreateClientProfileCommand NewCandidateCommand(
        string email = "marko.petrovic@email.rs",
        string? phone = "+381601234567") =>
        new(
            Email: email,
            FirstName: "Marko",
            LastName: "Petrović",
            PhoneNumber: phone,
            Gender: Gender.Male,
            DateOfBirth: new DateTime(1995, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            City: "Beograd",
            EducationLevel: EducationLevel.Bachelor,
            InstitutionName: "Fakultet organizacionih nauka",
            EducationStartYear: 2014,
            EducationEndYear: 2018,
            YearsOfExperience: 3,
            Skills: "C#, React");

    private void SetupClientsDbSet(List<ClientEntity> clients)
    {
        var dbSetMock = clients.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Clients).Returns(dbSetMock.Object);
        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task Kreira_profil_za_novog_kandidata()
    {
        SetupClientsDbSet([]);
        ClientEntity? saved = null;
        _contextMock.Setup(c => c.Clients.Add(It.IsAny<ClientEntity>()))
            .Callback<ClientEntity>(c => { c.ClientId = 1; saved = c; });

        var result = await _handler.Handle(NewCandidateCommand(), CancellationToken.None);

        result.Email.Should().Be("marko.petrovic@email.rs");
        result.FirstName.Should().Be("Marko");
        result.City.Should().Be("Beograd");
        result.EducationLevel.Should().Be(EducationLevel.Bachelor);
        result.YearsOfExperience.Should().Be(3);

        saved.Should().NotBeNull();
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Normalizuje_email_pri_cuvanju()
    {
        SetupClientsDbSet([]);
        ClientEntity? saved = null;
        _contextMock.Setup(c => c.Clients.Add(It.IsAny<ClientEntity>()))
            .Callback<ClientEntity>(c => saved = c);

        var result = await _handler.Handle(
            NewCandidateCommand(email: "  MARKO@Email.RS  "),
            CancellationToken.None);

        result.Email.Should().Be("marko@email.rs");
        saved!.Email.Should().Be("marko@email.rs");
    }

    [Fact]
    public async Task Dozvoljava_profil_bez_telefona()
    {
        SetupClientsDbSet([]);

        var result = await _handler.Handle(
            NewCandidateCommand(phone: null),
            CancellationToken.None);

        result.PhoneNumber.Should().BeNull();
    }

    [Fact]
    public async Task Ne_dozvoljava_dupli_email()
    {
        SetupClientsDbSet(
        [
            new ClientEntity
            {
                ClientId = 1,
                Email = "marko.petrovic@email.rs",
                PasswordHash = string.Empty,
                FirstName = "Ana",
                LastName = "Anić",
                Gender = Gender.Female,
                IsActive = true
            }
        ]);

        var act = () => _handler.Handle(
            NewCandidateCommand(email: "MARKO.PETROVIC@email.rs"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Profil za ovaj email već postoji.");

        _contextMock.Verify(c => c.Clients.Add(It.IsAny<ClientEntity>()), Times.Never);
    }
}
