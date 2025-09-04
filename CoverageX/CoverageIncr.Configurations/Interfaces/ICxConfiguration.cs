namespace CoverageIncr.Configurations.Interfaces;

public interface ICxConfiguration
{
    object? GetReceiver(string receiverName, Type returnType);
}