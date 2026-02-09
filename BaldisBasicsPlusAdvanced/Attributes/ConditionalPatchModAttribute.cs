using BaldisBasicsPlusAdvanced.Compats;
using MTM101BaldAPI;
using System;

namespace BaldisBasicsPlusAdvanced.Attributes
{
    internal class ConditionalPatchModAttribute : ConditionalPatch
    {
        private Type type;

        public ConditionalPatchModAttribute(Type moduleType)
        {
            this.type = moduleType;
        }

        public override bool ShouldPatch()
        {
            for (int i = 0; i < IntegrationManager.Modules.Count; i++)
            {
                if (IntegrationManager.Modules[i].GetType() == type)
                {
                    return IntegrationManager.Modules[i].IsIntegrable();
                }
            }
            return false;
        }
    }
}
