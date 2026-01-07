// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.


using NUnit.Framework;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7
{
    [TestFixture("Step7.zap21")]
    public class TransferAreaSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
    {
        [Test]
        public void createNewTransferAreaWithSameIOAddressesFromExistingTransferArea()
        {
            // Find master and slave PLCs
            var leadingPlc = Project.Devices.First(x => x.Name == "PLC_S120Democase").DeviceItems.First(x => x.Name == "PLC_S120Democase");
            var followingPlc2 = Project.Devices.First(x => x.Name == "PLC_2").DeviceItems.First(x => x.Name == "PLC_2");

            // Get Network interfaces for master and slaves
            var nwItf_leadingPlc = leadingPlc.DeviceItems.First(x => x.GetService<NetworkInterface>() is NetworkInterface nwItf && nwItf != null && nwItf.InterfaceType == NetType.Ethernet).GetService<NetworkInterface>();
            var nwItf_followingPlc2 = followingPlc2.DeviceItems.First(x => x.GetService<NetworkInterface>() is NetworkInterface nwItf && nwItf != null && nwItf.InterfaceType == NetType.Ethernet).GetService<NetworkInterface>();

            // get existing transfer area to PLC1
            var existingTransferArea = nwItf_leadingPlc.MulticastableTransferAreas.FirstOrDefault(x => x.Name == "LeadToPlc1");


            // Create new Multicast Transfer area with same IO addresses on LeadPlc from existing transfer
            var newTransferAreaFromLeadToPlc2 = nwItf_followingPlc2.MulticastableTransferAreas.Create(existingTransferArea, existingTransferArea.Type);

            // Set Processimages
            var mcServoObNumber = 91;

            // Set ProcessImage for following PLC 2
            var address_plc2 = newTransferAreaFromLeadToPlc2.Addresses.First();
            var mcServoBlock_plc2 = (followingPlc2.GetService<SoftwareContainer>().Software as PlcSoftware).BlockGroup.Blocks.FirstOrDefault(x => x.Number == mcServoObNumber);
            if (mcServoBlock_plc2 != null)
            {
                var processImagePartNumber_plc2 = mcServoBlock_plc2.GetAttribute("ProcessImagePartNumber");
                address_plc2.SetAttribute("ProcessImage", Convert.ToInt32(processImagePartNumber_plc2));
            }

            // Set ProcessImage for Leading PLC
            var address_LeadPlc = newTransferAreaFromLeadToPlc2.PartnerTransferAreas.First().Addresses.First();
            var mcServoBlock_LeadPlc = (leadingPlc.GetService<SoftwareContainer>().Software as PlcSoftware).BlockGroup.Blocks.FirstOrDefault(x => x.Number == mcServoObNumber);
            if (mcServoBlock_LeadPlc != null)
            {
                var processImagePartNumber_LeadPlc = mcServoBlock_LeadPlc.GetAttribute("ProcessImagePartNumber");
                address_LeadPlc.SetAttribute("ProcessImage", Convert.ToInt32(processImagePartNumber_LeadPlc));

            }

            // Known Bug:
            // address_plc2.Parent; => Accessing the Parent of address leads to crash of TIA Portal; That is also the reason why OpnsExplorer crashes if you try to access the address

            //address_plc2.StartAddress = 1000; // set input-adress for plc2 if required

            var newTransferAreaFromPlc2ToLead = nwItf_followingPlc2.MulticastableTransferAreas.Create(nwItf_leadingPlc, TransferAreaType.DDX, "Plc2ToLead", 1);
        }
    }
}
