using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsApplication1
{
    public partial class Form1 : Form
    {
        Dictionary<CheckBox, Cell> field = new Dictionary<CheckBox, Cell>();
        int timeSpent = 0;
        int money = 100;
        public Form1()
        {
            InitializeComponent();
            foreach (CheckBox cb in panel1.Controls)
                field.Add(cb, new Cell());
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (sender as CheckBox);
            if (cb.Checked) Plant(cb);
            else Harvest(cb);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (CheckBox cb in panel1.Controls)
                NextStep(cb);
            updateTimeLabel(timeSpent++);
        }

        private void updateTimeLabel(int tick)
        {
            labelTimeSpent.Text = $"Time Spent: {tick}";
        }

        private void Plant(CheckBox cb)
        {
            if (isMoneyEnough(Actions.Plant))
            {
                field[cb].Plant();
                UpdateBox(cb);
                UpdateMoney(Actions.Plant);
            }
        }

        private void Harvest(CheckBox cb)
        {
            switch(field[cb].state)
            {
                case CellState.Planted:
                    UpdateMoney(Actions.GetPlanted);
                    break;

                case CellState.Green:
                    UpdateMoney(Actions.GetGreen);
                    break;

                case CellState.Immature:
                    UpdateMoney(Actions.GetImmature);
                    break;

                case CellState.Mature:
                    UpdateMoney(Actions.GetMature);
                    break;

                case CellState.Overgrow:
                    UpdateMoney(Actions.GetOvermature);
                    break;
            }
            field[cb].Harvest();
            UpdateBox(cb);
        }

        private void NextStep(CheckBox cb)
        {
            field[cb].NextStep();
            UpdateBox(cb);
        }

        private void UpdateBox(CheckBox cb)
        {
            Color c = Color.White;
            switch (field[cb].state)
            {
                case CellState.Planted: c = Color.Black;
                    break;
                case CellState.Green: c = Color.Green;
                    break;
                case CellState.Immature: c = Color.Yellow;
                    break;
                case CellState.Mature: c = Color.Red;
                    break;
                case CellState.Overgrow: c = Color.Brown;
                    break;
            }
            cb.BackColor = c;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            timer1.Interval = trackBar1.Value * 20;
            labelCurrentInterval.Text = $"Current Interval: {timer1.Interval}";
        }

        private void buttonRestart_Click(object sender, EventArgs e)
        {
            money = 100;
            timeSpent = 0;
            clearAllCells();
            updateTimeLabel(timeSpent);
            updateMoneyLabel(money);
        }

        private void clearAllCells()
        {
            foreach (Cell cell in field.Values)
                cell.Harvest();
        }
        private void UpdateMoney(Actions action)
        {
            switch(action)
            {
                case Actions.Plant:
                    money -= 2;
                    break;
                case Actions.GetPlanted:
                    money -= 5;
                    break;
                case Actions.GetGreen:
                    money += 0;
                    break;
                case Actions.GetImmature:
                    money += 3;
                    break;
                case Actions.GetMature:
                    money += 5;
                    break;
                case Actions.GetOvermature:
                    money -= 1;
                    break;
            }
            updateMoneyLabel(money);
        }

        private void updateMoneyLabel(int money)
        {
            labelMoneyAvailable.Text = $"Money Available: {money}";
        }

        private bool isMoneyEnough(Actions action)
        {
            switch (action)
            {
                case Actions.Plant:
                    return money >= 2;
                case Actions.GetPlanted:
                    return money >= 5;
                case Actions.GetOvermature:
                    return money >= 1;
            }
            return true;
        }
    }



    enum CellState
    {
        Empty,
        Planted,
        Green,
        Immature,
        Mature,
        Overgrow
    }

    enum Actions
    {
        Plant,
        GetPlanted,
        GetGreen,
        GetImmature,
        GetMature,
        GetOvermature
    }

    class Cell
    {
        public CellState state = CellState.Empty;
        public int progress = 0;

        private const int prPlanted = 20;
        private const int prGreen = 100;
        private const int prImmature = 120;
        private const int prMature = 140;

        public void Plant()
        {
            state = CellState.Planted;
            progress = 1;
        }

        public void Harvest()
        {
            state = CellState.Empty;
            progress = 0;
        }

        public void NextStep()
        {
            if ((state != CellState.Empty) && (state != CellState.Overgrow))
            {
                progress++;
                if (progress < prPlanted) state = CellState.Planted;
                else if (progress < prGreen) state = CellState.Green;
                else if (progress < prImmature) state = CellState.Immature;
                else if (progress < prMature) state = CellState.Mature;
                else state = CellState.Overgrow;
            }
        }
    }
}