namespace Classes.Combat
{
    public enum Icon
    {
        Normal,
        Attack,
    }


    public class CombatIcon
    {
        DaxBlock normal = null;
        DaxBlock attack = null;

        // cheat and cache the flipped copy, so we don't damage the original, then we present this one later.
        DaxBlock normal_f = null;
        DaxBlock attack_f = null;

        public void Release()
        {
            normal = null;
            normal_f = null;
            attack = null;
            attack_f = null;
        }

        private static DaxBlock LoadIconHelper(int maskColor, bool shouldMask, int block_id, string file_text)
        {
            var data = Classes.DaxFiles.DaxCache.LoadDax(file_text + ".dax", block_id);
            return new DaxBlock(data, shouldMask, maskColor);
        }


        public void LoadIcons(int maskColor, bool shouldMask, string file_text, int normal_id, int attack_id)
        {
            normal = LoadIconHelper(maskColor, shouldMask, normal_id, file_text);
            normal_f = LoadIconHelper(maskColor, shouldMask, normal_id, file_text);
            normal_f.FlipIconLeftToRight();
            attack = LoadIconHelper(maskColor, shouldMask, attack_id, file_text);
            attack_f = LoadIconHelper(maskColor, shouldMask, attack_id, file_text);
            attack_f.FlipIconLeftToRight();
        }

        public void Recolor(bool p, byte[] newColors, byte[] oldColors)
        {
            normal.Recolor(p, newColors, oldColors);
            normal_f.Recolor(p, newColors, oldColors);
            attack.Recolor(p, newColors, oldColors);
            attack_f.Recolor(p, newColors, oldColors);
        }

        public DaxBlock GetIcon(Icon iconState, int direction)
        {
            if (iconState == Icon.Normal)
            {
                return direction > 3 ? normal_f : normal;
            }
            else
            {
                return direction > 3 ? attack_f : attack;
            }
        }

        public void MergeIcon(CombatIcon combatIcon) // used to blend head ad body icons.
        {
            normal.MergeIcons(combatIcon.normal);
            normal_f.MergeIcons(combatIcon.normal_f);
            attack.MergeIcons(combatIcon.attack);
            attack_f.MergeIcons(combatIcon.attack_f);
        }

        public void DuplicateIcon(bool Recolour, CombatIcon combatIcon, Player player)
        {
            int bitPerPixel = normal.Bpp;

            System.Array.Copy(combatIcon.normal.ImageData, normal.ImageData, combatIcon.normal.ImageData.Length);
            System.Array.Copy(combatIcon.normal_f.ImageData, normal_f.ImageData, combatIcon.normal_f.ImageData.Length);
            System.Array.Copy(combatIcon.attack.ImageData, attack.ImageData, combatIcon.attack.ImageData.Length);
            System.Array.Copy(combatIcon.attack_f.ImageData, attack_f.ImageData, combatIcon.attack_f.ImageData.Length);

            if (Recolour)
            {
                byte[] newColors = new byte[16];
                byte[] oldColors = new byte[16];

                for (byte i = 0; i < 16; i++)
                {
                    oldColors[i] = i;
                    newColors[i] = i;
                }

                for (byte i = 0; i < 6; i++)
                {
                    newColors[gbl.default_icon_colours[i]] = (byte) (player.icon_colours[i] & 0x0F);
                    newColors[gbl.default_icon_colours[i] + 8] = (byte) ((player.icon_colours[i] & 0xF0) >> 4);
                }

                Recolor(false, newColors, oldColors);
            }
        }
    }
}