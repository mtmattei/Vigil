using Vigil.Presentation;

namespace Vigil.Tests;

public class SignOffTests
{
    [Test]
    public void OnlyARecoveryCaseCanBeSigned()
    {
        var day = SampleDay.Create(DateTimeOffset.Now);
        SignOffSummary.From(day.Single(c => c.Status == CaseStatus.Recovery)).CanSign.Should().BeTrue();
        var live = SignOffSummary.From(day.Single(c => c.Status == CaseStatus.Anesthetized));
        live.CanSign.Should().BeFalse();
        live.Blocker.Should().Be("End anesthesia before signing.");
        SignOffSummary.From(day.Single(c => c.IsSigned)).Blocker.Should().StartWith("Already signed by");
    }

    [Test]
    public void CountsUseSingularForOne()
    {
        var milo = SampleDay.Create(DateTimeOffset.Now).Single(c => c.Patient.Name == "Milo");
        var oneDose = milo with { Doses = [new DoseGiven(Guid.NewGuid(), DateTimeOffset.Now, "x", "X", 1, 1, 1, DoseRoute.IV, false)] };
        SignOffSummary.From(oneDose).Counts.Should().StartWith("1 dose ·");
    }

    [Test]
    public void RecoveryDraftMapsPainIndexToScore()
    {
        var log = new RecoveryDraft(3, "37,2", " ok ").ApplyTo(RecoveryLog.Empty);
        log.PainScore.Should().Be(2);
        log.TempC.Should().Be(37.2m);
        log.Notes.Should().Be("ok");
        new RecoveryDraft(0, "", "").ApplyTo(log).PainScore.Should().BeNull();
        RecoveryDraft.From(log).PainIndex.Should().Be(3);
    }
}
