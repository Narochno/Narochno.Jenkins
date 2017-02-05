namespace Narochno.Jenkins.Entities.Jobs
{
    public class HealthReport
    {
        public string Description { get; set; }
        public int Score { get; set; }

        public override string ToString() => Description;
    }
}
