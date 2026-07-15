using ClientEntity = JobLess.Client.Domain.Entities.Client;
using FluentAssertions;
using JobLess.Client.Application.Clients.Commands.CreateClientProfile;
using JobLess.Client.Application.Clients.Commands.UpdateClientProfile;
using JobLess.Client.Application.Clients.Queries.GetClientProfile;
using JobLess.Client.Application.Interfaces;
using JobLess.Client.Domain.Enums;
using MockQueryable.Moq;
using Moq;

namespace JobLess.Tests.Client;


public class ClientProfileIntegrationTests
{
    private readonly Mock<IApplicationDbContext> _contextMock = new();
    private readonly List<ClientEntity> _clients = new();

    private readonly CreateClientProfileCommandHandler _createHandler;
    private readonly UpdateClientProfileCommandHandler _updateHandler;
    private readonly GetClientProfileQueryHandler _getHandler;

    public ClientProfileIntegrationTests()
    {
        _createHandler = new CreateClientProfileCommandHandler(_contextMock.Object);
        _updateHandler = new UpdateClientProfileCommandHandler(_contextMock.Object);
        _getHandler = new GetClientProfileQueryHandler(_contextMock.Object);
        RefreshDbSet();
    }

    private void RefreshDbSet()
    {
        var dbSetMock = _clients.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Clients).Returns(dbSetMock.Object);
        _contextMock.Setup(c => c.Clients.Add(It.IsAny<ClientEntity>()))
            .Callback<ClientEntity>(c =>
            {
                c.ClientId = _clients.Count + 1;
                _clients.Add(c);
            });
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    }

    [Fact]
    public async Task Kandidat_azurira_info_i_promena_je_vidljiva_nakon_get()
    {
        // 1) Kreiranje profila
        var createResult = await _createHandler.Handle(
            new CreateClientProfileCommand(
                Email: "marko.petrovic@email.rs",
                FirstName: "Marko",
                LastName: "Petrović",
                PhoneNumber: "+381601234567",
                Gender: Gender.Male,
                DateOfBirth: new DateTime(1995, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                City: "Novi Sad",
                EducationLevel: EducationLevel.Bachelor,
                InstitutionName: "FTN",
                EducationStartYear: 2014,
                EducationEndYear: 2018,
                YearsOfExperience: 2,
                Skills: "Java"),
            CancellationToken.None);
        RefreshDbSet();

        createResult.ClientId.Should().BeGreaterThan(0);
        createResult.City.Should().Be("Novi Sad");
        createResult.Skills.Should().Be("Java");

        var clientId = createResult.ClientId;
        var emailBefore = createResult.Email;

        // 2) UPDATE — menja se grad i veštine (i ostala polja profila po modelu update komande)
        await _updateHandler.Handle(
            new UpdateClientProfileCommand(
                ClientId: clientId,
                FirstName: "Marko",
                LastName: "Petrović",
                PhoneNumber: "+381601234567",
                Gender: Gender.Male,
                DateOfBirth: new DateTime(1995, 3, 15, 0, 0, 0, DateTimeKind.using ClientEntity = JobLess.Client.Domain.Entities.Client;
        using FluentAssertions;
        using JobLess.Client.Application.Clients.Commands.CreateClientProfile;
        using JobLess.Client.Application.Clients.Commands.UpdateClientProfile;
        using JobLess.Client.Application.Clients.Queries.GetClientProfile;
        using JobLess.Client.Application.Interfaces;
        using JobLess.Client.Domain.Enums;
        using MockQueryable.Moq;
        using Moq;

namespace JobLess.Tests.Client;

public class ClientProfileIntegrationTests
{
    private readonly Mock<IClientDbContext> _contextMock = new ();
    private readonly List<ClientEntity> _clients = new ();

    private readonly CreateClientProfileCommandHandler _createHandler;
    private readonly UpdateClientProfileCommandHandler _updateHandler;
    private readonly GetClientProfileQueryHandler _getHandler;

    public ClientProfileIntegrationTests()
    {
        _createHandler = new CreateClientProfileCommandHandler(_contextMock.Object);
        _updateHandler = new UpdateClientProfileCommandHandler(_contextMock.Object);
        _getHandler = new GetClientProfileQueryHandler(_contextMock.Object);

        RefreshDbSet();
    }

    private void RefreshDbSet()
    {
        var dbSetMock = _clients.AsQueryable().BuildMockDbSet();

        _contextMock
            .Setup(c => c.Clients)
            .Returns(dbSetMock.Object);

        _contextMock
            .Setup(c => c.Clients.Add(It.IsAny<ClientEntity>()))
            .Callback<ClientEntity>(client =>
            {
                client.ClientId = _clients.Count + 1;
                _clients.Add(client);
            });

        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task Should_Return_Updated_Client_Profile_After_Update()
    {
        var createdProfile = await _createHandler.Handle(
            new CreateClientProfileCommand(
                Email: "marko.petrovic@email.rs",
                FirstName: "Marko",
                LastName: "Petrović",
                PhoneNumber: "+381601234567",
                Gender: Gender.Male,
                DateOfBirth: new DateTime(1995, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                City: "Novi Sad",
                EducationLevel: EducationLevel.Bachelor,
                InstitutionName: "FTN",
                EducationStartYear: 2014,
                EducationEndYear: 2018,
                YearsOfExperience: 2,
                Skills: "Java"),
            CancellationToken.None);

        RefreshDbSet();

        createdProfile.ClientId.Should().BeGreaterThan(0);
        createdProfile.City.Should().Be("Novi Sad");
        createdProfile.Skills.Should().Be("Java");

        var clientId = createdProfile.ClientId;
        var email = createdProfile.Email;

        await _updateHandler.Handle(
            new UpdateClientProfileCommand(
                ClientId: clientId,
                FirstName: "Marko",
                LastName: "Petrović",
                PhoneNumber: "+381601234567",
                Gender: Gender.Male,
                DateOfBirth: new DateTime(1995, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                City: "Beograd",
                Address: "Knez Mihailova 10",
                EducationLevel: EducationLevel.Bachelor,
                InstitutionName: "FTN",
                EducationStartYear: 2014,
                EducationEndYear: 2018,
                YearsOfExperience: 2,
                Skills: "C#, React",
                ProfessionalSummary: "Junior developer",
                LinkedInUrl: "https://linkedin.com/in/marko",
                IsActive: true),
            CancellationToken.None);

        RefreshDbSet();

        var profile = await _getHandler.Handle(
            new GetClientProfileQuery(clientId),
            CancellationToken.None);

        profile.Should().NotBeNull();
        profile!.City.Should().Be("Beograd");
        profile.Skills.Should().Be("C#, React");
        profile.Address.Should().Be("Knez Mihailova 10");
        profile.ProfessionalSummary.Should().Be("Junior developer");
        profile.LinkedInUrl.Should().Be("https://linkedin.com/in/marko");

        profile.Email.Should().Be(email);
        profile.FirstName.Should().Be("Marko");
        profile.LastName.Should().Be("Petrović");
        profile.EducationLevel.Should().Be(EducationLevel.Bachelor);
        profile.InstitutionName.Should().Be("FTN");
        profile.YearsOfExperience.Should().Be(2);
    }
}
tc),
                City: "Beograd",
                Address: "Knez Mihailova 10",
                EducationLevel: EducationLevel.Bachelor,
                InstitutionName: "FTN",
                EducationStartYear: 2014,
                EducationEndYear: 2018,
                YearsOfExperience: 2,
                Skills: "C#, React",
                ProfessionalSummary: "Junior developer",
                LinkedInUrl: "https://linkedin.com/in/marko",
                IsActive: true),
            CancellationToken.None);
        RefreshDbSet();

        // 3) GET — izmena je vidljiva; email ostaje isti
        var afterUpdate = await _getHandler.Handle(new GetClientProfileQuery(clientId), CancellationToken.None);

        afterUpdate.Should().NotBeNull();
        afterUpdate!.City.Should().Be("Beograd");
        afterUpdate.Skills.Should().Be("C#, React");
        afterUpdate.Address.Should().Be("Knez Mihailova 10");
        afterUpdate.ProfessionalSummary.Should().Be("Junior developer");
        afterUpdate.LinkedInUrl.Should().Be("https://linkedin.com/in/marko");

        afterUpdate.Email.Should().Be(emailBefore);
        afterUpdate.FirstName.Should().Be("Marko");
        afterUpdate.LastName.Should().Be("Petrović");
        afterUpdate.EducationLevel.Should().Be(EducationLevel.Bachelor);
        afterUpdate.InstitutionName.Should().Be("FTN");
        afterUpdate.YearsOfExperience.Should().Be(2);
    }
}
