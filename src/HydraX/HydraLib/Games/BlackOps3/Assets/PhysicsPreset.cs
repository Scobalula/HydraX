using HydraLib.Assets;
using PhilLibX.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Games
{
    public partial class BlackOps3
    {
        public class PhysicsPreset
        {
            /// <summary>
            /// Bo3 Raw File Header
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct PhysicsPresetHeader
            {
                /// <summary>
                /// Pointer to the name of this raw file
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }

                /// <summary>
                /// Object Mass in Pounds
                /// </summary>
                public float Mass { get; set; }

                /// <summary>
                /// How much this object will bounce
                /// </summary>
                public float Bounce { get; set; }

                /// <summary>
                /// Object Friction
                /// </summary>
                public float Friction { get; set; }

                /// <summary>
                /// Linear Damping Scale
                /// </summary>
                public float LinearDamping { get; set; }

                /// <summary>
                /// Angular Damping Scale
                /// </summary>
                public float AngularDamping { get; set; }

                /// <summary>
                /// Bullet Force Scale
                /// </summary>
                public float BulletForceScale { get; set; }

                /// <summary>
                /// Explisve Force Scale
                /// </summary>
                public float ExplosiveForceScale { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private long Padding1 { get; set; }

                /// <summary>
                /// If this entity can float
                /// </summary>
                public int CanFloat { get; set; }

                /// <summary>
                /// Graity Scale
                /// </summary>
                public float GravityScale { get; set; }

                /// <summary>
                /// Mass Offset
                /// </summary>
                public Vector3 MassOffset { get; set; }

                /// <summary>
                /// Buoyancy Force Min
                /// </summary>
                public Vector3 BuoyancyMin { get; set; }

                /// <summary>
                /// Buoyancy Force Max
                /// </summary>
                public Vector3 BuoyancyMax { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding2 { get; set; }

                /// <summary>
                /// Trail FX Played on Object
                /// </summary>
                public long TrailFXPointer { get; set; }

                /// <summary>
                /// Object Impact Effects
                /// </summary>
                public long ImpactFXTablePointer { get; set; }

                /// <summary>
                /// Object Impact Sounds
                /// </summary>
                public long ImpactSoundsTablePointer { get; set; }
            }

            /// <summary>
            /// Loads the Assets for this type from Memory
            /// </summary>
            public static void Load(AssetPool assetPool)
            {
                for (int i = 0; i < assetPool.Size; i++)
                {
                    // Read Asset
                    var physPresetHeader = Hydra.ActiveGameReader.ReadStruct<PhysicsPresetHeader>(assetPool.FirstEntry + (i * assetPool.HeaderSize));
                    // Check is it a null/empty slot
                    if (assetPool.IsNullAsset(physPresetHeader.NamePointer))
                        continue;
                    // Create new asset
                    Hydra.LoadedAssets.Add(new Asset()
                    {
                        Name           = Hydra.ActiveGameReader.ReadNullTerminatedString(physPresetHeader.NamePointer),
                        HeaderAddress  = assetPool.FirstEntry + (i * assetPool.HeaderSize),
                        ExportFunction = Export,
                        Type           = assetPool.Name,
                        Information    = "N/A"
                    });
                }
            }

            /// <summary>
            /// Exports the given asset from Memory
            /// </summary>
            public static bool Export(Asset asset)
            {
                // Read Header
                var physPresetHeader = Hydra.ActiveGameReader.ReadStruct<PhysicsPresetHeader>(asset.HeaderAddress);
                // Check name pointer, if it's changed, our asset has changed
                if (asset.Name != Hydra.ActiveGameReader.ReadNullTerminatedString(physPresetHeader.NamePointer))
                    return false;
                // Add it to our GDTs
                Hydra.GDTs["Physic"].AddAsset(asset.Name, "physpreset", new PhysicsPresetObj()
                {
                    Bounce              = Math.Round(physPresetHeader.Bounce, 4),
                    Mass                = Math.Round(physPresetHeader.Mass * 1000.0, 4),
                    Friction            = Math.Round(physPresetHeader.Friction, 4),
                    LinearDamping       = Math.Round(physPresetHeader.LinearDamping, 4),
                    AngularDamping      = Math.Round(physPresetHeader.AngularDamping, 4),
                    BulletForceScale    = Math.Round(physPresetHeader.BulletForceScale, 4),
                    ExplosiveForceScale = Math.Round(physPresetHeader.ExplosiveForceScale, 4),
                    GravityScale        = Math.Round(physPresetHeader.GravityScale, 4),
                    MassOffsetX         = physPresetHeader.MassOffset.X,
                    MassOffsetY         = physPresetHeader.MassOffset.Y,
                    MassOffsetZ         = physPresetHeader.MassOffset.Z,
                    BuoyancyMinX        = physPresetHeader.BuoyancyMin.X,
                    BuoyancyMinY        = physPresetHeader.BuoyancyMin.Y,
                    BuoyancyMinZ        = physPresetHeader.BuoyancyMin.Z,
                    BuoyancyMaxX        = physPresetHeader.BuoyancyMax.X,
                    BuoyancyMaxY        = physPresetHeader.BuoyancyMax.Y,
                    BuoyancyMaxZ        = physPresetHeader.BuoyancyMax.Z,
                    CanFloat            = physPresetHeader.CanFloat,
                    TrailFX             = Hydra.CleanFXName(GetAssetName(physPresetHeader.TrailFXPointer)),
                    ImpactFXTable       = GetAssetName(physPresetHeader.ImpactFXTablePointer),
                    ImpactSoundTable    = GetAssetName(physPresetHeader.ImpactSoundsTablePointer)
                });
                // Done
                return true;
            }
        }
    }
}
