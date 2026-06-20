using WeatherBotDomain;

namespace WeatherBotDomainTests
{
    [TestFixture]
    public class TimeSegmnet_shouldcs
    {
        [Test]
        public void Test1()
        {
            var t1 = new TimeSegment(new TimeOnly(0, 0), new TimeOnly(4, 0));
            var t2 = new TimeSegment(new TimeOnly(6, 0), new TimeOnly(8, 0));
            var t3 = new TimeSegment(new TimeOnly(1, 0), new TimeOnly(2, 0));
            var t4 = new TimeSegment(new TimeOnly(3, 0), new TimeOnly(9, 0));
            var t5 = new TimeSegment(new TimeOnly(11, 0), new TimeOnly(13, 0));
            var t6 = new TimeSegment(new TimeOnly(12, 0), new TimeOnly(14, 0));

            var result = TimeSegment.Join(new[] { t1, t2, t3, t4, t5, t6 }).ToList();
        }
    }
}