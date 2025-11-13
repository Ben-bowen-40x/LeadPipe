namespace LeadPipe.Application.DataInterfaces.Entity;

public interface ISubsEntity
{
    ICalliEntity? CalliEntity { get; set; }
    long? CalliPhoneNumber { get; set; }
    DateTime CustDate { get; set; }
    int Id { get; set; }
    ILabEntity? LabEntity { get; set; }
    long? LabPhoneNumber { get; set; }
    ILeafEntity? LeafEntity { get; set; }
    long? LeafPhoneNumber { get; set; }
    long PhoneNumber { get; set; }
    DateTime SubDate { get; set; }
    IYellerEntity? YellerEntity { get; set; }
    long? YellerPhoneNumber { get; set; }
}