namespace ApplicationCore.DTOs;

public record PageResultDto<T>(IReadOnlyList<T> Items, int Total);
