namespace Template.Cli.Verbs;

internal interface IVerb
{
    public int Run(IServiceProvider service);
}
