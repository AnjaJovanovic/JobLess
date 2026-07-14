using FluentAssertions;
using JobLess.Company.Application.Commands.Create;
using JobLess.Company.Application.Commands.Update;
using JobLess.Company.Application.Common.Helpers;
using JobLess.Company.Application.Interfaces;
using JobLess.Company.Application.Queries.GetOne;
using JobLess.Company.Domain.Entities;
using JobLess.Company.Domain.Enums;
using JobLess.Shared.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using CompanyEntity = JobLess.Company.Domain.Entities.Company;

namespace JobLess.Tests.Company;

/// <summary>
/// Integracioni tok: registracija kompanije → GET (delatnost) → UPDATE delatnosti → GET
/// (ostala polja ostaju nepromenjena).
/// </summary>
public class CompanyProfileIntegrationTests
{
    private readonly Mock<IApplicationDbContext> _contextMock = new();
    private readonly Mock<IValidationExceptionThrower> _validationThrowerMock = new();
    private readonly List<CompanyEntity> _companies = new();

    private readonly CreateCompanyCommandHandler _createHandler;
    private readonly GetOneCompanyQueryHandler _getOneHandler;
    private readonly UpdateCompanyCommandHandler _updateHandler;

    public CompanyProfileIntegrationTests()
    {
        _createHandler = new CreateCompanyCommandHandler(_contextMock.Object, _validationThrowerMock.Object);
        _getOneHandler = new GetOneCompanyQueryHandler(_contextMock.Object);
        _updateHandler = new UpdateCompanyCommandHandler(_contextMock.Object);
        RefreshDbSet();
    }

    private void RefreshDbSet()
    {
        var dbSetMock = _companies.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Companies).Returns(dbSetMock.Object);
        _contextMock.Setup(c => c.Companies.Add(It.IsAny<CompanyEntity>()))
            .Callback<CompanyEntity>(c =>
            {
                c.Id = _companies.Count + 1;
                _companies.Add(c);
            });

        var adminDbSetMock = new Mock<DbSet<CompanyAdmin>>();
        _contextMock.Setup(c => c.CompanyAdmins).Returns(adminDbSetMock.Object);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    }

    private static CreateCompanyCommand ValidCreateCommand() => new()
    {
        Name = "TehnoBit DOO",
        Industry = "Informacione tehnologije",
        Location = "Beograd",
        Description = "Softverska kuća",
        Website = "www.tehnobit.rs",
        OwnerId = 1,
        OwnerName = "Petar Petrović",
        TaxIdentificationNumber = "123456789",
        RegistrationNumber = "12345678",
        ContactPersonFirstName = "Ana",
        ContactPersonLastName = "Anić",
        ContactPersonPosition = "HR",
        ContactPersonPhoneNumber = "+381601234567",
        Email = "hr@tehnobit.rs",
        PhoneNumber = "+381111234567",
        Address = "Bulevar kralja Aleksandra 1",
        CompanySize = CompanySize.OneToTen
    };

    [Fact]
    public async Task Kompanija_se_registruje_get_vraca_delatnost_update_menja_samo_delatnost()
    {
        // 1) Registracija / unos podataka
        var createCommand = ValidCreateCommand();
        var companyId = await _createHandler.Handle(createCommand, CancellationToken.None);
        RefreshDbSet();

        companyId.Should().BeGreaterThan(0);

        // 2) GET — provera vraćenih podataka (uključujući delatnost)
        var afterCreate = await _getOneHandler.Handle(new GetOneCompanyQuery { Id = companyId }, CancellationToken.None);

        afterCreate.Should().NotBeNull();
        afterCreate!.Company.Should().NotBeNull();
        afterCreate.Company.Name.Should().Be("TehnoBit DOO");
        afterCreate.Company.Industry.Should().Be(Industry.InformationTechnology);
        afterCreate.Company.Location.Should().Be("Beograd");
        afterCreate.Company.Description.Should().Be("Softverska kuća");
        afterCreate.Company.Website.Should().Be("www.tehnobit.rs");
        afterCreate.Company.Email.Should().Be("hr@tehnobit.rs");
        afterCreate.Company.OwnerName.Should().Be("Petar Petrović");
        afterCreate.Company.Address.Should().Be("Bulevar kralja Aleksandra 1");

        var snapshotBeforeUpdate = afterCreate.Company;

        // 3) UPDATE — menja se samo delatnost
        var updated = await _updateHandler.Handle(new UpdateCompanyCommand
        {
            CompanyId = companyId,
            CompanyEmail = "hr@tehnobit.rs",
            Industry = Industry.Healthcare
        }, CancellationToken.None);

        updated.Should().BeTrue();
        RefreshDbSet();

        // 4) GET — delatnost promenjena, ostalo isto
        var afterUpdate = await _getOneHandler.Handle(new GetOneCompanyQuery { Id = companyId }, CancellationToken.None);

        afterUpdate.Should().NotBeNull();
        afterUpdate!.Company.Industry.Should().Be(Industry.Healthcare);

        afterUpdate.Company.Name.Should().Be(snapshotBeforeUpdate.Name);
        afterUpdate.Company.Location.Should().Be(snapshotBeforeUpdate.Location);
        afterUpdate.Company.Description.Should().Be(snapshotBeforeUpdate.Description);
        afterUpdate.Company.Website.Should().Be(snapshotBeforeUpdate.Website);
        afterUpdate.Company.Email.Should().Be(snapshotBeforeUpdate.Email);
        afterUpdate.Company.OwnerName.Should().Be(snapshotBeforeUpdate.OwnerName);
        afterUpdate.Company.Address.Should().Be(snapshotBeforeUpdate.Address);
        afterUpdate.Company.ContactPersonFirstName.Should().Be(snapshotBeforeUpdate.ContactPersonFirstName);
        afterUpdate.Company.ContactPersonLastName.Should().Be(snapshotBeforeUpdate.ContactPersonLastName);
        afterUpdate.Company.PhoneNumber.Should().Be(snapshotBeforeUpdate.PhoneNumber);
        afterUpdate.Company.CompanySize.Should().Be(snapshotBeforeUpdate.CompanySize);
    }
}
