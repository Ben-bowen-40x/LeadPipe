using CSharpFunctionalExtensions;
using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Data.Transform;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Entity.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Repository.Sqlite;
using LeadPipe.Infrastructure.Interfaces.Translate;
using LeadPipe.Infrastructure.Settings;
using NSubstitute;
using System.Linq.Expressions;

namespace LeadPipe.Infrastructure.Test.ReportTransformationTests;

public class TransformYellerReportTests
{
    #region Ctor and Fields
    private readonly IRepositoryFactory _repoFactory = Substitute.For<IRepositoryFactory>();
    private readonly IRepository<CustardEntity> _custardRepo = Substitute.For<IRepository<CustardEntity>>();
    private readonly IRepository<CaliperEntity> _caliperRepo = Substitute.For<IRepository<CaliperEntity>>();
    private readonly IRepository<CornEntity> _cornRepo = Substitute.For<IRepository<CornEntity>>();
    private readonly IRepository<PlumbingEntity> _plumbRepo = Substitute.For<IRepository<PlumbingEntity>>();

    private readonly IEntityToReport<PlumbingEntity, ReportYeller> _plumbToR = Substitute.For<IEntityToReport<PlumbingEntity, ReportYeller>>();
    private readonly IEntityToReport<CaliperEntity, ReportYeller> _caliperToR = Substitute.For<IEntityToReport<CaliperEntity, ReportYeller>>();
    private readonly IEntityToReport<CornEntity, ReportYeller> _cornToR = Substitute.For<IEntityToReport<CornEntity, ReportYeller>>();

    private readonly IEntityToReport<CustardPlumbingLink, ReportYeller> _cpLinkToR = Substitute.For<IEntityToReport<CustardPlumbingLink, ReportYeller>>();
    private readonly IEntityToReport<CustardCaliperLink, ReportYeller> _custCalToR = Substitute.For<IEntityToReport<CustardCaliperLink, ReportYeller>>();
    private readonly IEntityToReport<CustardCornLink, ReportYeller> _custCornToR = Substitute.For<IEntityToReport<CustardCornLink, ReportYeller>>();

    private readonly IYellerSettings _settings = Substitute.For<IYellerSettings>();

    public TransformYellerReportTests()
    {
        _repoFactory.GetRepository<CustardEntity>().Returns(_custardRepo);
        _repoFactory.GetRepository<CaliperEntity>().Returns(_caliperRepo);
        _repoFactory.GetRepository<CornEntity>().Returns(_cornRepo);
        _repoFactory.GetRepository<PlumbingEntity>().Returns(_plumbRepo);

        _settings.YellerCaliperSource1.Returns("Cal1");
        _settings.YellerCaliperSource2.Returns("Cal2");
        _settings.YellerCornSource.Returns("Corn1");
    }
    #endregion

    [Fact]
    public async Task TransformAsync_WithNoData_ReturnsEmptyList()
    {
        // Arrange
        _custardRepo.FindWithDetailsAsync(Arg.Any<Expression<Func<CustardEntity, bool>>>())
            .Returns(Result.Success(new List<CustardEntity>()));

        _caliperRepo.FindAsync(Arg.Any<Expression<Func<CaliperEntity, bool>>>())
            .Returns(Result.Success(new List<CaliperEntity>()));

        _cornRepo.FindAsync(Arg.Any<Expression<Func<CornEntity, bool>>>())
            .Returns(Result.Success(new List<CornEntity>()));

        _plumbRepo.FindAsync(Arg.Any<Expression<Func<PlumbingEntity, bool>>>())
            .Returns(Result.Success(new List<PlumbingEntity>()));

        var transformer = new TransformYellerReport(
            _repoFactory, _plumbToR, _caliperToR, _cornToR,
            _cpLinkToR, _custCalToR, _custCornToR, _settings
        );

        // Act
        var result = await transformer.TransformAsync(new List<Plumbing>());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task TransformAsync_NonCustardEntities_Translated()
    {
        // Arrange
        long numb = 5551000001;
        var calipers = new List<CaliperEntity>
            {
                new CaliperEntity { Id = 1, PhoneNumber = new PhoneNumber(numb), Note="note", Location="loc", Source="Cal1", Date = DateTime.Now }
            };
        var corns = new List<CornEntity>();
        var plumbs = new List<PlumbingEntity>();

        _custardRepo.FindWithDetailsAsync(Arg.Any<Expression<Func<CustardEntity, bool>>>())
            .Returns(Result.Success(new List<CustardEntity>()));
        _caliperRepo.FindAsync(Arg.Any<Expression<Func<CaliperEntity, bool>>>())
            .Returns(Result.Success(calipers));
        _cornRepo.FindAsync(Arg.Any<Expression<Func<CornEntity, bool>>>())
            .Returns(Result.Success(corns));
        _plumbRepo.FindAsync(Arg.Any<Expression<Func<PlumbingEntity, bool>>>())
            .Returns(Result.Success(plumbs));

        _caliperToR.Translate(Arg.Any<CaliperEntity>())
            .Returns(ci => new ReportYeller
            {
                event_id = "evt",
                event_time = 1,
                user_data = new UserData { ph = new[] { $"{numb}" } },
                custom_data = new CustomData { value = 1m, currency = "USD" }
            });

        var transformer = new TransformYellerReport(
            _repoFactory, _plumbToR, _caliperToR, _cornToR,
            _cpLinkToR, _custCalToR, _custCornToR, _settings
        );

        // Act
        var result = await transformer.TransformAsync(new List<Plumbing>
            {
                new Plumbing(1, new PhoneNumber(numb), DateTimeOffset.Now, null, null, "meta", Source.Yeller)
            });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task TransformAsync_CustardWithMultipleLinks_ResolvesEarliestAttributable()
    {
        // Arrange
        long numb = 5551234567;
        var custard = new CustardEntity
        {
            PhoneNumber = new PhoneNumber(numb),
            Id = 1,
            Date = new DateTime(2026, 1, 1),
            SandEntities = new List<SandEntity>(),
            CustardCaliperLinks = new List<CustardCaliperLink>
                {
                    new CustardCaliperLink { CustardId = 1, CaliperId = 1, MatchingPhone = numb, UnixMatchDate = 1000 },
                    new CustardCaliperLink { CustardId = 1, CaliperId = 2, MatchingPhone = numb, UnixMatchDate = 1001 }
                }
        };

        var calipers = new List<CaliperEntity>
            {
                new CaliperEntity
                {
                    Id = 1,
                    PhoneNumber = new PhoneNumber(numb),
                    Note = "note1",
                    Location = "loc1",
                    Source = "Caliper1",
                    Date = new DateTime(2025, 12, 31)
                },
                new CaliperEntity
                {
                    Id = 2,
                    PhoneNumber = new PhoneNumber(numb),
                    Note = "note2",
                    Location = "loc2",
                    Source = "Caliper1",
                    Date = new DateTime(2026, 1, 2)
                }
            };

        _custardRepo.FindWithDetailsAsync(Arg.Any<Expression<Func<CustardEntity, bool>>>())
            .Returns(Result.Success(new List<CustardEntity> { custard }));

        _caliperRepo.FindAsync(Arg.Any<Expression<Func<CaliperEntity, bool>>>())
            .Returns(Result.Success(calipers));

        _cornRepo.FindAsync(Arg.Any<Expression<Func<CornEntity, bool>>>())
            .Returns(Result.Success(new List<CornEntity>()));

        _plumbRepo.FindAsync(Arg.Any<Expression<Func<PlumbingEntity, bool>>>())
            .Returns(Result.Success(new List<PlumbingEntity>()));

        _custCalToR.Translate(Arg.Any<CustardCaliperLink>())
            .Returns(ci => new ReportYeller
            {
                event_id = "evt",
                event_time = 1,
                user_data = new UserData { ph = new[] { $"{numb}" } },
                custom_data = new CustomData { value = 1m, currency = "USD" }
            });

        var transformer = new TransformYellerReport(
            _repoFactory, _plumbToR, _caliperToR, _cornToR,
            _cpLinkToR, _custCalToR, _custCornToR, _settings
        );

        // Act
        var result = await transformer.TransformAsync(new List<Plumbing>
            {
                new Plumbing(1, new PhoneNumber(numb), DateTimeOffset.Now, null, null, "meta", Source.Yeller)
            });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }
    [Fact]
    public async Task TransformAsync_SandDate_PreventsAttributable()
    {
        // Arrange
        long numb = 5551234567;
        var custard = new CustardEntity
        {
            PhoneNumber = new PhoneNumber(numb),
            Id = 1,
            Date = new DateTime(2026, 1, 1),
            SandEntities = new List<SandEntity>
                {
                    new SandEntity { Id=1, CustardId=1, Offerman="1000" }
                },
            CustardCaliperLinks = new List<CustardCaliperLink>
                {
                    new CustardCaliperLink { CustardId=1, CaliperId=1, MatchingPhone=numb, UnixMatchDate=1000 }
                }
        };
        var calipers = new List<CaliperEntity>
            {
                new CaliperEntity { Id=1, PhoneNumber=new PhoneNumber(numb), Note="n1", Location="loc1", Source="Cal1", Date=new DateTime(2026,1,2) }
            };

        _custardRepo.FindWithDetailsAsync(Arg.Any<Expression<Func<CustardEntity, bool>>>())
            .Returns(Result.Success(new List<CustardEntity> { custard }));
        _caliperRepo.FindAsync(Arg.Any<Expression<Func<CaliperEntity, bool>>>())
            .Returns(Result.Success(calipers));
        _cornRepo.FindAsync(Arg.Any<Expression<Func<CornEntity, bool>>>())
            .Returns(Result.Success(new List<CornEntity>()));
        _plumbRepo.FindAsync(Arg.Any<Expression<Func<PlumbingEntity, bool>>>())
            .Returns(Result.Success(new List<PlumbingEntity>()));

        _caliperToR.Translate(Arg.Any<CaliperEntity>())
            .Returns(ci => new ReportYeller
            {
                event_id = "evt",
                event_time = 1,
                user_data = new UserData { ph = new[] { $"{numb}" } },
                custom_data = new CustomData { value = 1m, currency = "USD" }
            });

        var transformer = new TransformYellerReport(
            _repoFactory, _plumbToR, _caliperToR, _cornToR,
            _cpLinkToR, _custCalToR, _custCornToR, _settings
        );

        // Act
        var result = await transformer.TransformAsync(new List<Plumbing>
            {
                new Plumbing(1, new PhoneNumber(numb), DateTimeOffset.Now, null, null, "meta", Source.Yeller)
            });

        // Assert
        // Sand date is later, so the caliper is non-attributable
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }
}
