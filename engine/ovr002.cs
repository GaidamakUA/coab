using System;
using Classes;

namespace engine
{
    class ovr002
    {
        static void delay_or_key(int seconds)
        {
            seg043.clear_keyboard();

            var timeEnd = DateTime.Now.AddSeconds(seconds);

            while (seg049.KEYPRESSED() == false &&
                   DateTime.Now < timeEnd)
            {
                System.Threading.Thread.Sleep(100);
            }

            seg043.clear_keyboard();
        }


        static void credits()
        {
            Display.UpdateStop();

            seg037.draw8x8_02();

            Seg041.DisplayString("based on the tsr novel 'azure bonds'", 0, 10, 1, 2);
            Seg041.DisplayString("by:", 0, 10, 2, 6);
            Seg041.DisplayString("kate novak", 0, 11, 2, 9);
            Seg041.DisplayString("and", 0, 10, 2, 0x14);
            Seg041.DisplayString("jeff grubb", 0, 11, 2, 0x18);
            Seg041.DisplayString("scenario created by:", 0, 10, 4, 0x0a);
            Seg041.DisplayString("tsr, inc.", 0, 0x0e, 5, 0x0b);
            Seg041.DisplayString("and", 0, 0x0a, 5, 0x15);
            Seg041.DisplayString("ssi", 0, 0x0e, 5, 0x19);
            Seg041.DisplayString("jeff grubb", 0, 0x0b, 6, 0x0e);
            Seg041.DisplayString("george mac donald", 0x0, 0x0B, 0x7, 0x0B);
            Seg041.DisplayString("game created by:", 0x0, 0x0A, 0x9, 0x1);
            Seg041.DisplayString("ssi special projects", 0x0, 0x0E, 0x9, 0x12);
            Seg041.DisplayString("project leader:", 0x0, 0x0E, 0x0B, 0x2);
            Seg041.DisplayString("george mac donald", 0x0, 0x0B, 0x0C, 0x2);
            Seg041.DisplayString("programming:", 0x0, 0x0E, 0x0E, 0x2);
            Seg041.DisplayString("scot bayless", 0x0, 0x0B, 0x0F, 0x2);
            Seg041.DisplayString("russ brown", 0x0, 0x0B, 0x10, 0x2);
            Seg041.DisplayString("michael mancuso", 0x0, 0x0B, 0x11, 0x2);
            Seg041.DisplayString("development:", 0x0, 0x0E, 0x13, 0x2);
            Seg041.DisplayString("david shelley", 0x0, 0x0B, 0x14, 0x2);
            Seg041.DisplayString("michael mancuso", 0x0, 0x0B, 0x15, 0x2);
            Seg041.DisplayString("oran kangas", 0x0, 0x0B, 0x16, 0x2);
            Seg041.DisplayString("graphic arts:", 0x0, 0x0E, 0x0B, 0x16);
            Seg041.DisplayString("tom wahl", 0x0, 0x0B, 0x0C, 0x16);
            Seg041.DisplayString("fred butts", 0x0, 0x0B, 0x0D, 0x16);
            Seg041.DisplayString("susan manley", 0x0, 0x0B, 0x0E, 0x16);
            Seg041.DisplayString("mark johnson", 0x0, 0x0B, 0x0F, 0x16);
            Seg041.DisplayString("cyrus lum", 0x0, 0x0B, 0x10, 0x16);
            Seg041.DisplayString("playtesting:", 0x0, 0x0E, 0x12, 0x16);
            Seg041.DisplayString("jim jennings", 0x0, 0x0B, 0x13, 0x16);
            Seg041.DisplayString("james kucera", 0x0, 0x0B, 0x14, 0x16);
            Seg041.DisplayString("rick white", 0x0, 0x0B, 0x15, 0x16);
            Seg041.DisplayString("robert daly", 0x0, 0x0B, 0x16, 0x16);

            Display.UpdateStart();
        }


        internal static void title_screen()
        {
            DaxBlock dax_ptr;

            dax_ptr = seg040.LoadDax(0, 0, 1, "Title");
            seg040.draw_picture(dax_ptr, 0, 0, 0);

            delay_or_key(5);

            dax_ptr = seg040.LoadDax(0, 0, 2, "Title");
            seg040.draw_picture(dax_ptr, 0, 0, 0);

            dax_ptr = seg040.LoadDax(0, 0, 3, "Title");
            seg040.draw_picture(dax_ptr, 0x0b, 6, 0);
            delay_or_key(10);

            dax_ptr = seg040.LoadDax(0, 0, 4, "Title");

            seg044.PlaySound(Sound.sound_d);

            seg040.draw_picture(dax_ptr, 0x0b, 0, 0);
            delay_or_key(10);

            Seg041.ClearScreen();
            credits();
            delay_or_key(10);

            Seg041.ClearScreen();
        }
    }
}