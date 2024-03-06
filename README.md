Project for reading the 2box .dsnd files and extract the samples from it,
and generating .dsnd from samples.

**Generate dsnd files:**
For example, the directory structure is as follows:

d:\samples\Gold 20 Kick\zone3_Head\
  contains: Gold 20 Kick_0.wav
            Gold 20 Kick_1.wav
            Gold 20 Kick_2.wav
            ect

d:\samples\Snare 909 Type 1\
contains
    zone8_Rim\Snare 909 Type 1_0.wav
    zone8_Rim\Snare 909 Type 1_1.wav
    zone8_Rim\Snare 909 Type 1_2.wav
    ect...
    zone3_Snare\Snare 909 Type 1_0.wav
    zone3_Snare\Snare 909 Type 1_1.wav
    zone3_Snare\Snare 909 Type 1_3.wav
    ect...


Dsnd.CLI.exe -g "d:\samples\Gold 20 Kick" -o
   produces "d:\samples\Gold 20 Kick\Gold 20 Kick.dsnd" in the current directory, overwriting any exstisting "Gold 20 Kick.dsnd" with one zone name "zone3_Head"

Dsnd.CLI.exe -g "d:\samples\Gold 20 Kick" -o -p "d:\outputsamples"
  produces "d:\outputsamples\Gold 20 Kick.dsnd", overwriting the exstisting "Gold 20 Kick.dsnd" with one zone name "zone3_Head"

Dsnd.CLI.exe -g "d:\samples" -o
  process all subdirs in "d:\samples" and writes in the directory preceding the zones directories the .dsnd file.
  



