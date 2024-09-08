namespace GilGoblin.Database.Pocos;

public record WorldUploadTimeDbPoco(int Id, int ItemId, int WorldId, bool IsHq, long Timestamp) : WorldUploadTimeWebPoco(WorldId, Timestamp);