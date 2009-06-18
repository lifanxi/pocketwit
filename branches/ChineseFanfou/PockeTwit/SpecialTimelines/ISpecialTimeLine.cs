namespace PockeTwit
{
    public interface ISpecialTimeLine
    {
        string ListName { get; }
        string name { get; set; }
        string GetConstraints();
        string ToString();
        SpecialTimelines.SpecialTimeLinesRepository.TimeLineType Timelinetype { get; }
    }
}