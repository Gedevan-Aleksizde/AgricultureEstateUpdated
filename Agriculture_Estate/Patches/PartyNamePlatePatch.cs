﻿// Decompiled with JetBrains decompiler
// Type: AgricultureEstate.PartyNamePlatePatch
// Assembly: AgricultureEstate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A4103D21-1273-439E-B48D-A11FAB56D6B9
// Assembly location: C:\Users\andre\Downloads\AgricultureEstate\bin\Win64_Shipping_Client\AgricultureEstate.dll

using HarmonyLib;
using SandBox.ViewModelCollection.Nameplate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace AgricultureEstate
{
    [HarmonyPatch(typeof(SettlementNameplateVM), "RefreshBindValues")]
    class PartyNamePlatePatch
    {
        private static void Postfix(SettlementNameplateVM __instance)
        {
            if (!__instance.Settlement.IsVillage)
                return;
            if (!HasTwo(__instance.SettlementEvents.EventsList) && OwnsLand(__instance.Settlement))
            {
                string stringId = __instance.Settlement.Village.VillageType.PrimaryProduction.StringId;
                string str = stringId.Contains("camel") ? "camel" : (stringId.Contains("horse") || stringId.Contains("mule") ? "horse" : stringId);
                __instance.SettlementEvents.EventsList.Add(new SettlementNameplateEventItemVM(str));
            }
            else
            {
                if (!HasTwo(__instance.SettlementEvents.EventsList) || OwnsLand(__instance.Settlement))
                    return;
                SettlementNameplateEventItemVM nameplateEventItemVm = __instance.SettlementEvents.EventsList.FirstOrDefault<SettlementNameplateEventItemVM>(e => e.EventType == SettlementNameplateEventItemVM.SettlementEventType.Production);
                __instance.SettlementEvents.EventsList.Remove(nameplateEventItemVm);
            }
        }

        private static bool OwnsLand(Settlement settlement)
        {
            if (AgricultureEstateBehavior.VillageLands == null)
            {
                AgricultureEstateBehavior.VillageLands = new Dictionary<Settlement, VillageLand>();
                return false;
            }
            VillageLand villageLand;
            return AgricultureEstateBehavior.VillageLands.TryGetValue(settlement, out villageLand) && (villageLand.OwnedPlots > 0 || villageLand.OwnedUndevelopedPlots > 0);
        }

        private static bool HasTwo(
          MBBindingList<SettlementNameplateEventItemVM> eventsList)
        {
            int num = 0;
            foreach (SettlementNameplateEventItemVM events in (Collection<SettlementNameplateEventItemVM>)eventsList)
            {
                if (events.EventType == SettlementNameplateEventItemVM.SettlementEventType.Production)
                    ++num;
            }
            return num >= 2;
        }
    }
}
