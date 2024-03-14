using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using static HarmonyLib.AccessTools;

namespace RecycleRate
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            var harmony = new Harmony("localghost.RecycleRateAdjustment");
            harmony.Patch(EnumeratorMoveNext(Method("Verse.Thing:SmeltProducts")), transpiler: new HarmonyMethod(Method("RecycleRate.HarmonyPatches:Transpiler")));
            if (Contains("notfood.mendandrecycle"))
                harmony.Patch(Method("MendAndRecycle.JobDriverUtils:Reclaim"), transpiler: new HarmonyMethod(Method("RecycleRate.HarmonyPatches:ReclaimTranspiler")));
        }
        static bool Contains(string packageId) => ModLister.GetActiveModWithIdentifier(packageId, true) != null;
    }

    public class HarmonyPatches
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R4)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, typeof(RecycleRateMod).GetField(nameof(RecycleRateMod.settings)));
                    yield return new CodeInstruction(OpCodes.Ldfld, typeof(Settings).GetField(nameof(Settings.recycleRate)));
                }
                else
                    yield return instruction;
            }
        }

        static IEnumerable<CodeInstruction> ReclaimTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldarg_1)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, typeof(RecycleRateMod).GetField(nameof(RecycleRateMod.settings)));
                    yield return new CodeInstruction(OpCodes.Ldfld, typeof(Settings).GetField(nameof(Settings.recycleRate)));
                }
                else
                    yield return instruction;
            }
        }
    }
}
