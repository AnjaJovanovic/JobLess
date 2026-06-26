using ClientEntity = JobLess.Client.Domain.Entities.Client;
using FluentAssertions;
using JobLess.Client.Application.Clients.Commands.UpdateClientProfile;
using JobLess.Client.Application.Interfaces;
using JobLess.Client.Domain.Enums;
using MockQueryable.Moq;
using Moq;

namespace JobLess.Tests.Client;

public class UpdateClientProfileCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly UpdateClientProfileCommandHandler _handler;

    public UpdateClientProfileCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new UpdateClientProfileCommandHandler(_contextMock.Object);
    }

    private static ClientEntity PostojeciKandidat(int clientId = 1) =>
        new()
        {
            ClientId = clientId,
            Email = "marko.petrovic@email.rs",
            PasswordHash = string.Empty,
            FirstName = "Marko",
            LastName = "Petrović",
            PhoneNumber = "+381601234567",
            Gender = Gender.Male,
            City = "Novi Sad",
            YearsOfExperience = 1,
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc)
        };

    private static UpdateClientProfileCommand AzuriraniProfil(int clientId = 1) =>
        new(
            ClientId: clientId,
            FirstName: "Jovana",
            LastName: "Jović",
            PhoneNumber: "+381611112222",
            Gender: Gender.Female,
            DateOfBirth: new DateTime(1995, 5, 15, 0, 0, 0, DateTimeKind.Utc),
            City: "Beograd",
            Address: "Knez Mihailova 1",
            EducationLevel: EducationLevel.Bachelor,
            InstitutionName: "Fakultet organizacionih nauka",
            EducationStartYear: 2014,
            EducationEndYear: 2018,
            YearsOfExperience: 5,
            Skills: "C#, React",
            ProfessionalSummary: "Iskusan developer.",
            LinkedInUrl: "https://linkedin.com/in/jovana",
            IsActive: true);

    private void SetupClientsDbSet(List<ClientEntity> clients)
    {
        var dbSetMock = clients.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Clients).Returns(dbSetMock.Object);
        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task Azurira_profil_postojeceg_kandidata()
    {
        var kandidat = PostojeciKandidat();
        SetupClientsDbSet([kandidat]);
        var command = AzuriraniProfil();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.FirstName.Should().Be("Jovana");
        result.LastName.Should().Be("Jović");
        result.City.Should().Be("Beograd");
        result.EducationLevel.Should().Be(EducationLevel.Bachelor);
        result.YearsOfExperience.Should().Be(5);
        result.Skills.Should().Be("C#, React");

        kandidat.FirstName.Should().Be("Jovana");
        kandidat.InstitutionName.Should().Be("Fakultet organizacionih nauka");
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Ne_menja_email_pri_azuriranju()
    {
        var kandidat = PostojeciKandidat();
        var originalCreatedAt = kandidat.CreatedAt;
        SetupClientsDbSet([kandidat]);

        var result = await _handler.Handle(AzuriraniProfil(), CancellationToken.None);

        result.Email.Should().Be("marko.petrovic@email.rs");
        kandidat.Email.Should().Be("marko.petrovic@email.rs");
        kandidat.CreatedAt.Should().Be(originalCreatedAt);
    }

    [Fact]
    public async Task Baca_gresku_kada_kandidat_ne_postoji()
    {
        SetupClientsDbSet([]);

        var act = () => _handler.Handle(AzuriraniProfil(clientId: 999), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Klijent sa ID 999 nije pronađen.");

        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Menja_samo_profil_trazenog_kandidata()
    {
        var marko = PostojeciKandidat(1);
        var ana = new ClientEntity
        {
            ClientId = 2,
            Email = "ana.anic@email.rs",
            PasswordHash = string.Empty,
            FirstName = "Ana",
            LastName = "Anić",
            Gender = Gender.Female,
            IsActive = true,
            CreatedAt = new DateTime(2025, 2, 1, 10, 0, 0, DateTimeKind.Utc)
        };

        SetupClientsDbSet([marko, ana]);

        await _handler.Handle(AzuriraniProfil(clientId: 1), CancellationToken.None);

        marko.FirstName.Should().Be("Jovana");
        ana.FirstName.Should().Be("Ana");
    }
}
