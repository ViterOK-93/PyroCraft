using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using Microsoft.Win32;

partial class PyroCraft {
    private RegistryKey[] preset = new RegistryKey[PresetCount];
    
    private enum MachineType {
        LaserEngraver,
        NichromeBurner,
        ImpactGraver,
    }
    
    private const int PresetFileSize = ((12 + 4*26 + 3) / 2 * 2);
    
    private string presetName = "";
    private MachineType machineType = MachineType.LaserEngraver;
    
    private unsafe void LoadPreset(int idx) {
        if (idx == -2) {
            goto SKIP_PRESET_LOAD;
        }
        
        Func<string, float, float> ReadFloat;
        Func<string, int, int> ReadByte;
        
        ReadFloat = preset[idx].GetSingle;
        ReadByte = preset[idx].GetInt32;
        
        machineType = (MachineType)ReadByte("MachineType", (int)MachineType.LaserEngraver);
        gcG0Speed = ReadFloat("G0Speed", 12000F);
        gcRotarySpeed = ReadFloat("RotarySpeed", 1500F);
        gcAccel = ReadFloat("Accel", 3000F);
        gcNichromeOnOffCommand = (NichromeControl)ReadByte("NichromeOnOffCommand", (int)NichromeControl.Default_M3_M5);
        gcBurnFromBottomToTop = (ReadByte("BurnFromBottomToTop", 0) != 0);
        gcDontReturnY = (ReadByte("DontReturnY", 1) != 0);
        
        textBox5.Text = ReadFloat("Speed", 1200F).ToString();
        textBox6.Text = ReadFloat("Power", 255F).ToString();
        
        comboBox9.Text = ReadByte("NumberOfPasses", 1).ToString();
        textBox9.Text = ReadFloat("HeatDelay", 7F).ToString();
        checkBox12.Checked = (ReadByte("AirAssist", 0) != 0);
        
        checkBox6.Checked = (ReadByte("SkipWhite", 1) != 0);
        textBox10.Text = ReadFloat("WhiteSpeed", 4000F).ToString();
        textBox23.Text = ReadFloat("WhiteDistance", 5F).ToString();
        
        Func<float, float> constrain_0_1 = (n) => {
            if (n >= 1F) {
                return 1F;
            }
            if (n <= 0F) {
                return 0F;
            }
            return n;
        };
        
        textBox11.Text = ReadFloat("SpeedGraphMax", 2820F).ToString();
        textBox12.Text = ReadFloat("SpeedGraphMin", 350F).ToString();
        gcSpeedGraph[2] = constrain_0_1(ReadFloat("SpeedGraphPt1X", 0.35F));
        gcSpeedGraph[3] = constrain_0_1(ReadFloat("SpeedGraphPt1Y", 0.35F));
        gcSpeedGraph[4] = constrain_0_1(ReadFloat("SpeedGraphPt2X", 0.65F));
        gcSpeedGraph[5] = constrain_0_1(ReadFloat("SpeedGraphPt2Y", 0.65F));
        panel1.Invalidate(false);
        
        textBox13.Text = ReadFloat("PowerGraphMax", 255F).ToString();
        textBox14.Text = ReadFloat("PowerGraphMin", 255F).ToString();
        gcPowerGraph[2] = constrain_0_1(ReadFloat("PowerGraphPt1X", 0.35F));
        gcPowerGraph[3] = constrain_0_1(ReadFloat("PowerGraphPt1Y", 0.35F));
        gcPowerGraph[4] = constrain_0_1(ReadFloat("PowerGraphPt2X", 0.65F));
        gcPowerGraph[5] = constrain_0_1(ReadFloat("PowerGraphPt2Y", 0.65F));
        panel2.Invalidate(false);
        
        checkBox7.Checked = (ReadByte("Bidirectional", 1) != 0);
        
        comboBox5.SelectedItem = (CleaningStrategy)ReadByte("CleaningStrategy", (int)CleaningStrategy.Always);
        comboBox6.Text = ReadByte("CleaningRowsCount", 2).ToString();
        textBox17.Text = ReadFloat("StripWidth", 20F).ToString();
        textBox18.Text = ReadFloat("StripSpeed", 1000F).ToString();
        textBox19.Text = ReadFloat("CleaningFieldWidth", 5F).ToString();
        textBox20.Text = ReadFloat("CleaningFieldSpeed", 5000F).ToString();
        comboBox8.Text = ReadByte("NumberOfCleaningCycles", 2).ToString();
        
        checkBox3.Checked = (ReadByte("WrappedOutput", 0) != 0);
        textBox7.Text = ReadFloat("MmPerRevolution", 360F).ToString();
        textBox8.Text = ReadFloat("CylinderDiameter", 50F).ToString();
        
        textBox22.Text = preset[idx].GetString("PresetName", ("Preset" + (1+idx).ToString(invariantCulture)));
        
        SKIP_PRESET_LOAD:
        bool isNichromeBurner = (machineType == MachineType.NichromeBurner);
        bool isImpactGraver = (machineType == MachineType.ImpactGraver);
        bool isLaserEngraver = !(isNichromeBurner || isImpactGraver);
        
        resetSpeedGraphToolStripMenuItem.Visible = !isImpactGraver;
        resetPowerGraphToolStripMenuItem.Visible = !isNichromeBurner;
        rotarySpeedToolStripMenuItem.Visible = isLaserEngraver;
        accelToolStripMenuItem.Visible = !isNichromeBurner;
        nichromeOnOffCommandToolStripMenuItem.Visible = isNichromeBurner;
        doNotReturnYToolStripMenuItem.Visible = isNichromeBurner;
        
        tableLayoutPanel4.SuspendLayout2();
        tableLayoutPanel29.Visible = isImpactGraver;
        tableLayoutPanel18.Visible = isNichromeBurner;
        checkBox12.Visible = isLaserEngraver;
        checkBox6.Visible = !isNichromeBurner;
        label44.Visible = !isNichromeBurner;
        textBox23.Visible = !isNichromeBurner;
        tableLayoutPanel20.Visible = !isImpactGraver;
        tableLayoutPanel21.Visible = !isNichromeBurner;
        tableLayoutPanel26.Visible = isNichromeBurner;
        tableLayoutPanel11.Visible = isLaserEngraver;
        tableLayoutPanel4.ResumeLayout2();
        
        label21.Enabled = (im1bitPalette || isImpactGraver);
        textBox5.Enabled = (im1bitPalette || isImpactGraver);
        if (isNichromeBurner) {
            label22.Enabled = (gcNichromeOnOffCommand < NichromeControl.OUT2_M8_M9);
            textBox6.Enabled = (gcNichromeOnOffCommand < NichromeControl.OUT2_M8_M9);
        } else {
            label22.Enabled = im1bitPalette;
            textBox6.Enabled = im1bitPalette;
        }
        tableLayoutPanel19.Enabled = (gcSkipWhite || isNichromeBurner);
        
        bWorkerFlags = (BWorkerFlags.CalcJobTime|BWorkerFlags.RedrawOrigin);
    }
    
    private unsafe void SavePreset(int idx) {
        Action<string, float> WriteFloat;
        Action<string, object> WriteBoolean;
        Action<string, object> WriteByte;
        
        WriteFloat = preset[idx].SetSingle;
        WriteBoolean = preset[idx].SetInt32;
        WriteByte = preset[idx].SetInt32;
        
        WriteByte("MachineType", machineType);
        WriteFloat("G0Speed", gcG0Speed);
        WriteFloat("RotarySpeed", gcRotarySpeed);
        WriteFloat("Accel", gcAccel);
        WriteByte("NichromeOnOffCommand", gcNichromeOnOffCommand);
        WriteBoolean("BurnFromBottomToTop", gcBurnFromBottomToTop);
        WriteBoolean("DontReturnY", gcDontReturnY);
        
        WriteFloat("Speed", gcSpeed);
        WriteFloat("Power", gcPower);
        
        WriteByte("NumberOfPasses", gcNumberOfPasses);
        WriteFloat("HeatDelay", gcHeatDelay);
        WriteBoolean("AirAssist", gcAirAssist);
        
        WriteBoolean("SkipWhite", gcSkipWhite);
        WriteFloat("WhiteSpeed", gcWhiteSpeed);
        WriteFloat("WhiteDistance", gcWhiteDistance);
        
        WriteFloat("SpeedGraphMax", gcSpeedGraph[1]);
        WriteFloat("SpeedGraphMin", gcSpeedGraph[0]);
        WriteFloat("SpeedGraphPt1X", gcSpeedGraph[2]);
        WriteFloat("SpeedGraphPt1Y", gcSpeedGraph[3]);
        WriteFloat("SpeedGraphPt2X", gcSpeedGraph[4]);
        WriteFloat("SpeedGraphPt2Y", gcSpeedGraph[5]);
        
        WriteFloat("PowerGraphMax", gcPowerGraph[1]);
        WriteFloat("PowerGraphMin", gcPowerGraph[0]);
        WriteFloat("PowerGraphPt1X", gcPowerGraph[2]);
        WriteFloat("PowerGraphPt1Y", gcPowerGraph[3]);
        WriteFloat("PowerGraphPt2X", gcPowerGraph[4]);
        WriteFloat("PowerGraphPt2Y", gcPowerGraph[5]);
        
        WriteBoolean("Bidirectional", gcBidirectional);
        
        WriteByte("CleaningStrategy", gcCleaningStrategy);
        WriteByte("CleaningRowsCount", gcCleaningRowsCount);
        WriteFloat("StripWidth", gcStripWidth);
        WriteFloat("StripSpeed", gcStripSpeed);
        WriteFloat("CleaningFieldWidth", gcCleaningFieldWidth);
        WriteFloat("CleaningFieldSpeed", gcCleaningFieldSpeed);
        WriteByte("NumberOfCleaningCycles", gcNumberOfCleaningCycles);
        
        WriteBoolean("WrappedOutput", gcWrappedOutput);
        WriteFloat("MmPerRevolution", mmPerRevolution);
        WriteFloat("CylinderDiameter", cylinderDiameter);
        
        preset[idx].SetString("PresetName", presetName);
        preset[idx].Flush();
    }
}
