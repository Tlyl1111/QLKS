using HotelManager.DAO;
using System;
using System.Windows.Forms;

namespace HotelManager
{
    public partial class fAddRoomType : Form
    {
        public delegate void RoomTypeAddedHandler();
        public event RoomTypeAddedHandler RoomTypeAdded;

        public fAddRoomType()
        {
            InitializeComponent();
        }

        private void btnClose__Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAddRoomType_Click(object sender, EventArgs e)
        {
            string name = txbNameRoomType.Text.Trim();
            string priceText = txbPrice.Text.Trim();
            string limitPersonText = txbLimitPerson.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(priceText) || string.IsNullOrEmpty(limitPersonText))
            {
                MessageBox.Show("Không được để trống", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(priceText, out int price))
            {
                MessageBox.Show("Giá phải là một số nguyên", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(limitPersonText, out int limitPerson))
            {
                MessageBox.Show("Số người giới hạn phải là một số nguyên", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                bool isSuccess = RoomTypeDAO.Instance.InsertRoomType(name, price, limitPerson);

                if (isSuccess)
                {
                    MessageBox.Show("Thêm loại phòng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RoomTypeAdded?.Invoke(); // Notify listeners about the new room type
                    this.Close(); // Close the form after successful insertion
                }
                else
                {
                    MessageBox.Show("Lỗi không thêm được loại phòng này", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
