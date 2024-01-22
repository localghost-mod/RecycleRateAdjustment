using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace RecycleRate
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup() => new Harmony("localghost.RecycleRateAdjustment").PatchAll();
    }

    [HarmonyPatch(typeof(Thing), nameof(Thing.SmeltProducts), MethodType.Enumerator)]
    public static class SmeltProductsPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R4)
                {
                    yield return new CodeInstruction(
                        OpCodes.Ldsfld,
                        typeof(RecycleRateMod).GetField(nameof(RecycleRateMod.settings))
                    );
                    yield return new CodeInstruction(
                        OpCodes.Ldfld,
                        typeof(Settings).GetField(nameof(Settings.recycleRate))
                    );
                }
                else
                    yield return instruction;
            }
        }
    }
}
