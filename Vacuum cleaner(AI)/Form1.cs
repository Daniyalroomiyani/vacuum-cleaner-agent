using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Vacuum_cleaner_AI_.Room;

namespace Vacuum_cleaner_AI_
{
    public partial class Form1 : Form
    {

        private Control[] controlList;
        private Room[] roomslist;
        private VacumCleaner vc;
        private int row, column;
        private Image trashimg;
        private Image vcimg, cleaningimg, roomimg;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cleaningimg = Image.FromFile("cleaning.png");
            trashimg = Image.FromFile("ecology-and-environment.png");
            

            vcimg = Image.FromFile("cleaning(3).png");
            roomimg = Image.FromFile("P6.png");

            controlList = new Control[pnlrooms.Controls.Count];
            pnlrooms.Controls.CopyTo(controlList, 0);

            btn.Enabled = false;
            TextChanged(null, null);
            txti.Focus();

            label3.Text += "\n( between 1 - 9)";

        }
        private void selectroom(object sender, EventArgs e)
        {

            if (roomslist[Convert.ToInt32((((PictureBox)sender).Name).Remove(0, 4))].Status == roomstatus.clean)
                roomslist[Convert.ToInt32((((PictureBox)sender).Name).Remove(0, 4))].Status = roomstatus.dirty;

            else if (roomslist[Convert.ToInt32((((PictureBox)sender).Name).Remove(0, 4))].Status == roomstatus.dirty)
                roomslist[Convert.ToInt32((((PictureBox)sender).Name).Remove(0, 4))].Status = roomstatus.clean;


            generaterooms();
        }

        private void btnshowrooms_Click(object sender, EventArgs e)
        {
            txti.Enabled = txtj.Enabled = false;
            btn.Text = "Finished";
            btn.Click += new EventHandler(btnsaverooms_Click);
            btn.Click -= btnshowrooms_Click;
            row = Convert.ToInt32(txti.Text);
            column = Convert.ToInt32(txtj.Text);
            vc = new VacumCleaner(row, column);


            vc.FlagH1 = true;
            vc.FlagV1 = false;




            roomslist = new Room[row * column];
            int left, top, h, w, temp;
            temp = 0;
            left = 0;
            top = 0;
            int c = 0;
            h = pnlrooms.Height / (row);
            w = pnlrooms.Width / (column);

            pnlrooms.Controls.Clear();

            for (int m = 0; m < row; m++)
            {
                for (int n = 0; n < column; ++n)
                {

                    PictureBox pic = new PictureBox();
                    pic.BackgroundImage = roomimg;
                    pic.SizeMode = PictureBoxSizeMode.StretchImage;
                    pic.Top = top;
                    pic.Height = h;
                    pic.Width = w;
                    pic.Left = left;
                    pic.Name = ("room" + c.ToString());
                    pic.Click += new EventHandler(selectroom);
                    roomslist[c] = new Room();
                    roomslist[c].Loc = new Location(m, n);                    
                    roomslist[c].Loc.RoomNum = c;
                    pnlrooms.Controls.Add(pic);
                    left += pic.Width;
                    temp = pic.Height;                   
                    c++;
                    pic.BackgroundImageLayout = ImageLayout.Stretch;
                }
                left = 0;
                top += temp;
            }

            label3.Text = "Please click on the rooms to put trash in them.... \nAnd then click on Finished ";
        
        }

        private void btnsaverooms_Click(object sender, EventArgs e)
        {


            btn.Text = "Start";
            btn.Click += new EventHandler(Start);
            btn.Click -= btnsaverooms_Click;

            label3.Text = "Choose a vacuum cleaner room to start..";    
            foreach (PictureBox btn in pnlrooms.Controls)
            {
                btn.Click += new EventHandler(selectVCLocation);
                btn.Click -= selectroom;
            }

            btn.Enabled = false;



        }





        private void generaterooms()
        {
            int a;
            Location l;          
            foreach (PictureBox p in pnlrooms.Controls)
            {
                a = Convert.ToInt32((p.Name).Remove(0, 4));
                l = roomslist[a].Loc;

                if (l.Equals(vc.VCLocation))
                    p.Image = vcimg;

                else if (roomslist[Convert.ToInt32((p.Name).Remove(0, 4))].Status == roomstatus.dirty)
                    p.Image = trashimg;
                else
                    p.Image = null;

                if (l.Equals(vc.VCLocation) && roomslist[a].Status == roomstatus.dirty)
                     p.Image = cleaningimg;              
                            
            }
        }


        private void selectVCLocation(object sender, EventArgs e)
        {
            int ish, jsh;
            ish = roomslist[Convert.ToInt32((((PictureBox)sender).Name).Remove(0, 4))].Loc.I;
            jsh = roomslist[Convert.ToInt32((((PictureBox)sender).Name).Remove(0, 4))].Loc.J;
            int numsh = roomslist[Convert.ToInt32((((PictureBox)sender).Name).Remove(0, 4))].Loc.RoomNum;
            Location l = new Location(ish, jsh);
            l.RoomNum = numsh;
            vc.VCLocation = l;
            
            vc.MovementNum = vc.MovementNum = column - (vc.VCLocation.J + 1);


            generaterooms();
            btn.Enabled = true;
            label3.Text = "Press Start to start the cleaning....";
            btn.Focus();


        }


        private void SendROomData()
        {
            vc.Sensor = roomslist[vc.VCLocation.RoomNum].Status;
        }



       private void MakeDelay( int delay)
        {
            //this.Invalidate();
            Thread.Sleep(delay);
            //this.Update();
            //this.Refresh();
        }

        private void Start(object sender, EventArgs e)
        {
            foreach (PictureBox p in pnlrooms.Controls)
                p.Click -= selectVCLocation;

            

            
            label3.Text = "Vacuum Cleaner is moving and cleaning .....";

            
            MethodInvoker updateSTBarDelegate = delegate () { lblSTBar.Text = vc.StatusBarText; };
            Task.Run(() =>
            {
                while (!IsAllChecked())
                {


                    for (int i = 0; i < vc.MovementNum; i++)
                    {

                        SendROomData();
                        roomslist[vc.VCLocation.RoomNum].Status = vc.ChekRoom();

                        lblSTBar.Text = vc.StatusBarText;

                        MakeDelay(600);

                        roomslist[vc.VCLocation.RoomNum].RoomIsChecked = true;
                        if (IsAllChecked())
                            break;

                        vc.MoveVc();

                        generaterooms();


                    }

                    vc.ReadyToMove();
                }
                generaterooms();


                lblSTBar.Text = "*****All Rooms are clean*****";
                
                MessageBox.Show("All rooms are checked and cleaned...", "Done", MessageBoxButtons.OK, MessageBoxIcon.None);
            });


            pnltools.Controls.RemoveAt(0);
            btnExit.Enabled = btnRestart.Enabled = true; 
        
           

        }

        private void Restart(object sender , EventArgs e)
        {
            Application.Restart();
        }

       
        private new void TextChanged(object sender, EventArgs e)
        {
            if (txti.Text != "" && txtj.Text != "")
                btn.Enabled = true;

        }


        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Button t = new Button();
            t.Text = e.KeyChar.ToString();
            if (e.KeyChar >= '1' && e.KeyChar < '9')
                SelectNumber(t, null);
            else if (e.KeyChar == '\b')
                BackSpace(null, null);
        }

        

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BackSpace(object sender, EventArgs e)
        {



            if (txti.Focused)
                if (txti.TextLength > 0)
                    txti.Text = txti.Text.Substring(0, txti.TextLength - 1);
            if (txtj.Focused)
                if (txtj.TextLength > 0)
                    txtj.Text = txtj.Text.Substring(0, txti.TextLength - 1);



        }


        private void SelectNumber(object sender, EventArgs e)
        {


            if (txti.Focused)
            {
                if (txti.SelectionLength > 0)
                    txti.Text = "";
                txti.Text += ((Button)sender).Text;
            }
            if (txtj.Focused)
            {
                if (txtj.SelectionLength > 0)
                    txtj.Text = "";
                txtj.Text += ((Button)sender).Text;
            }

        }


        private bool IsAllChecked()
        {
            bool z = true;
            foreach (Room r in roomslist)
                if (!r.RoomIsChecked)
                    z = false;
            return z;

        }



    }
}
    
