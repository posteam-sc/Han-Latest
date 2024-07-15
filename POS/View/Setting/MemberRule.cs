using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.APP_Data;
using System.Text.RegularExpressions;

namespace POS
{
    public partial class MemberRule : Form
    {
        #region Variables
        private bool isStart = false;

        private bool isEdit = false;
        int oldRangeFromAmount = 0;
        public int ruleID { get; set; }

        private POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();

        MemberCardRule currMember = new MemberCardRule();
        int currentId;
        #endregion

        #region Event
        public MemberRule()
        {
            InitializeComponent();
        }
        private void btnAddMember_Click_1(object sender, EventArgs e)
        {
            newMemberType newType = new newMemberType();
            newType.ShowDialog();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            if (cboMemberRule.Text == "Select")
            {
                tp.SetToolTip(cboMemberRule, "Error");
                tp.Show("Please select Member Type!", cboMemberRule);
            }
            else if (txtFrom.Text.Trim() == string.Empty || txtFrom.Text.Trim() == "0")
            {
                tp.SetToolTip(txtFrom, "Error");
                tp.Show("Please fill up Initial Amount!", txtFrom);
            }
            else if (rbtBetween.Checked && txtTo.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtTo, "Error");
                tp.Show("Please fill up Amount!", txtTo);
            }
            //else if (txtMemberDis.Text.Trim() == string.Empty || txtMemberDis.Text.Trim() == "0")
            //{
            //    tp.SetToolTip(txtMemberDis, "Error");
            //    tp.Show("Please fill up Member Card Discount!", txtMemberDis);
            //}
            //else if (txtBirthdayDis.Text.Trim() == string.Empty || txtBirthdayDis.Text.Trim() == "0")
            //{
            //    tp.SetToolTip(txtBirthdayDis, "Error");
            //    tp.Show("Please fill up Birthday Discount!", txtBirthdayDis);
            //}
            else if (rbtBetween.Checked && txtTo.Text.Trim() == "Above")
            {
                tp.SetToolTip(txtFrom, "Error");
                tp.Show("Please check your input Amount!", txtFrom);
            }
            else if (rbtBetween.Checked && Convert.ToInt32(txtFrom.Text.Trim()) >= Convert.ToInt32(txtTo.Text.Trim()))
            {
                tp.SetToolTip(txtTo, "Error");
                tp.Show("Please check your input Amount!", txtTo);
            }
            //else if (Convert.ToInt32(txtMemberDis.Text.Trim()) <= 0 || Convert.ToInt32(txtMemberDis.Text.Trim()) > 100)
            //{
            //    tp.SetToolTip(txtMemberDis, "Error");
            //    tp.Show("Please check your input for Member Discount!", txtMemberDis);
            //}
            //else if (Convert.ToInt32(txtBirthdayDis.Text.Trim()) <= 0 || Convert.ToInt32(txtBirthdayDis.Text.Trim()) > 100)
            //{
            //    tp.SetToolTip(txtBirthdayDis, "Error");
            //    tp.Show("Please check your input for Birthday Discount!", txtBirthdayDis);
            //}
            else
            {
                APP_Data.MemberCardRule mRule = new APP_Data.MemberCardRule();
                //Role Management
                RoleManagementController controller = new RoleManagementController();
                controller.Load(MemberShip.UserRoleId);

                //Add new rule
                if (!isEdit)
                {
                    if (controller.MemberRule.Add || MemberShip.isAdmin)
                    {
                        bool IsSuccessful = false;
                        string mType = cboMemberRule.Text;
                        int mTypeid = (from m in entity.MemberTypes where m.Name == mType select m.Id).SingleOrDefault();
                        string rFrom = txtFrom.Text.Trim();
                        string rTo = null;
                        if (rbtBetween.Checked)
                        {
                            rTo = txtTo.Text.Trim();
                        }
                        else
                        {
                            rTo = "Above";
                        }

                        // int count = entity.MemberCardRules.Where(x => x.IsActive == true).Count();
                        int count = entity.MemberCardRules.Where(x => x.IsActive == true && x.MemberTypeId == mTypeid).Count();
                        if (count == 0)
                        {
                            mRule.MemberTypeId = mTypeid;
                            mRule.RangeFrom = rFrom;
                            mRule.RangeTo = rTo;
                            mRule.MCDiscount = 0;
                            mRule.BDDiscount = 0;
                            mRule.IsCalculatePoints = chkCalculatePoint.Checked;
                            mRule.IsActive = chkIsActive.Checked;
                            mRule.CreatedBy = MemberShip.UserId;
                            mRule.CreatedDate = DateTime.Now;
                            entity.MemberCardRules.Add(mRule);
                            entity.SaveChanges();

                            Clear();
                            IsSuccessful = true;
                        }
                        else
                        {
                            MessageBox.Show("Member rule for current member type already exists.", "Active Member Rule Exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            Clear();
                            IsSuccessful = false;

                            //    #region OldConditionCode
                            //    //int minRF = (from m in entity.MemberCardRules.AsEnumerable()  where m.IsActive==true select Convert.ToInt32(m.RangeFrom)).Min();
                            //    //int minRT = 0;
                            //    //int countRange = (from m in entity.MemberCardRules where m.IsActive==true &&  m.RangeTo != "Above" select m).Count();
                            //    //if (countRange != 0)
                            //    //{
                            //    //    minRT = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive==true && m.RangeTo != "Above" select Convert.ToInt32(m.RangeTo)).Min();
                            //    //}
                            //    //string miniRT = (from m in entity.MemberCardRules where m.IsActive == true select m.RangeTo).Min();
                            //    //int checkRT;
                            //    //if (miniRT != "Above")
                            //    //{
                            //    //    checkRT = minRT;
                            //    //}
                            //    //else
                            //    //{
                            //    //    checkRT = minRF;
                            //    //}
                            //    //int maxiRT = 0;
                            //    //if (countRange != 0)
                            //    //{
                            //    //    maxiRT = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true && m.RangeTo != "Above" select Convert.ToInt32(m.RangeTo)).Max();
                            //    //}
                            //    //string maxRT = (from m in entity.MemberCardRules where m.IsActive == true select m.RangeTo).Max();
                            //    //if (maxRT != "Above")
                            //    //{
                            //    //    maxRT = maxiRT.ToString();
                            //    //}
                            //    //int maxRF = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true select Convert.ToInt32(m.RangeFrom)).Max();
                            //    //if (rbtBetween.Checked)
                            //    //{
                            //    //    if (Convert.ToInt32(rTo) < minRF)
                            //    //    {
                            //    //        mRule.MemberTypeId = mTypeid;
                            //    //        mRule.RangeFrom = rFrom;
                            //    //        mRule.RangeTo = rTo;
                            //    //        mRule.MCDiscount = mDis;
                            //    //        mRule.BDDiscount = bDis;
                            //    //        mRule.IsCalculatePoints = chkCalculatePoint.Checked;
                            //    //        mRule.IsActive = chkIsActive.Checked;
                            //    //        entity.MemberCardRules.Add(mRule);
                            //    //        entity.SaveChanges();
                            //    //        reloadMember();
                            //    //        reloadMemberList();
                            //    //        Clear();
                            //    //        IsSuccessful = true;
                            //    //    }
                            //    //    else if (count == 1 && maxRT != "Above" && Convert.ToInt32(rFrom) > checkRT)
                            //    //    {
                            //    //        mRule.MemberTypeId = mTypeid;
                            //    //        mRule.RangeFrom = rFrom;
                            //    //        mRule.RangeTo = rTo;
                            //    //        mRule.MCDiscount = mDis;
                            //    //        mRule.BDDiscount = bDis;
                            //    //        mRule.IsCalculatePoints = chkCalculatePoint.Checked;
                            //    //        mRule.IsActive = chkIsActive.Checked;
                            //    //        entity.MemberCardRules.Add(mRule);
                            //    //        entity.SaveChanges();
                            //    //        reloadMember();
                            //    //        reloadMemberList();
                            //    //        Clear();
                            //    //        IsSuccessful = true;
                            //    //    }
                            //    //    else if (Convert.ToInt32(rFrom) >= minRF && maxRF >= Convert.ToInt32(rTo))
                            //    //    {
                            //    //        List<APP_Data.MemberCardRule> cardList = new List<APP_Data.MemberCardRule>();
                            //    //        bool isCorrect = false;
                            //    //        cardList = (from m in entity.MemberCardRules  where m.IsActive==true orderby m.RangeTo ascending select m).ToList();
                            //    //        foreach (MemberCardRule mcr in cardList)
                            //    //        {
                            //    //            int mcrRF = Convert.ToInt32(mcr.RangeFrom);
                            //    //            int minimumRT = 0;
                            //    //            string minFrom = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true && m.Id != mcr.Id && (Convert.ToInt32(m.RangeFrom)) >= mcrRF select m.RangeFrom).Min();
                            //    //            if (minFrom != null)
                            //    //            {
                            //    //                minimumRT = Convert.ToInt32(minFrom);
                            //    //            }
                            //    //            else
                            //    //            {
                            //    //                minimumRT = 0;
                            //    //            }
                            //    //            if (mcr.RangeTo != "Above")
                            //    //            {
                            //    //                if (Convert.ToInt32(mcr.RangeTo) < Convert.ToInt32(rFrom) && minimumRT > Convert.ToInt32(rTo))
                            //    //                {
                            //    //                    mRule.MemberTypeId = mTypeid;
                            //    //                    mRule.RangeFrom = rFrom;
                            //    //                    mRule.RangeTo = rTo;
                            //    //                    mRule.MCDiscount = mDis;
                            //    //                    mRule.BDDiscount = bDis;
                            //    //                    mRule.IsCalculatePoints = chkCalculatePoint.Checked;
                            //    //                    mRule.IsActive = chkIsActive.Checked;
                            //    //                    entity.MemberCardRules.Add(mRule);
                            //    //                    entity.SaveChanges();
                            //    //                    reloadMember();
                            //    //                    reloadMemberList();
                            //    //                    Clear();
                            //    //                    IsSuccessful = true;
                            //    //                    isCorrect = false;
                            //    //                    break;
                            //    //                }
                            //    //                else
                            //    //                {
                            //    //                    isCorrect = true;
                            //    //                }
                            //    //            }
                            //    //        }
                            //    //        if (isCorrect == true)
                            //    //        {
                            //    //            MessageBox.Show("Please check your Amount!");
                            //    //        }
                            //    //    }
                            //    //    else
                            //    //    {
                            //    //        if (maxRT == "Above" && Convert.ToInt32(rFrom) > maxRF)
                            //    //        {
                            //    //            MessageBox.Show("Please check your Amount.You have already entered amount " + maxRF.ToString() + " - " + maxRT + " in one of your member type!");
                            //    //        }
                            //    //        else
                            //    //        {
                            //    //            MessageBox.Show("Please check your Amount!");
                            //    //        }
                            //    //    }
                            //    //}
                            //    //else
                            //    //{
                            //    //    if (maxRT == "Above")
                            //    //    {
                            //    //        MessageBox.Show("Please check your Amount.You have already entered amount \"" + maxRF.ToString() + " - " + maxRT + "\" in one of your member type!");
                            //    //    }
                            //    //    else
                            //    //    {
                            //    //        if (Convert.ToInt32(maxRT) < Convert.ToInt32(rFrom))
                            //    //        {
                            //    //            mRule.MemberTypeId = mTypeid;
                            //    //            mRule.RangeFrom = rFrom;
                            //    //            mRule.RangeTo = rTo;
                            //    //            mRule.MCDiscount = mDis;
                            //    //            mRule.BDDiscount = bDis;
                            //    //            mRule.IsCalculatePoints = chkCalculatePoint.Checked;
                            //    //            mRule.IsActive = chkIsActive.Checked;
                            //    //            entity.MemberCardRules.Add(mRule);
                            //    //            entity.SaveChanges();
                            //    //            reloadMember();
                            //    //            reloadMemberList();
                            //    //            Clear();
                            //    //            IsSuccessful = true;
                            //    //        }
                            //    //        else
                            //    //        {
                            //    //            MessageBox.Show("Please check your Amount!");
                            //    //        }
                            //    //    }
                            //    //}
                            //    #endregion

                            //    int minRF = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true select Convert.ToInt32(m.RangeFrom)).Min();
                            //    int minRT = 0;
                            //    int countRange = (from m in entity.MemberCardRules where m.IsActive == true && m.RangeTo != "Above" select m).Count();
                            //    if (countRange != 0)
                            //    {
                            //        minRT = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true && m.RangeTo != "Above" select Convert.ToInt32(m.RangeTo)).Min();
                            //    }
                            //    else
                            //    {
                            //        string miniRT = (from m in entity.MemberCardRules where m.IsActive == true select m.RangeTo).Min();
                            //    }

                            //    string status = "";
                            //    List<APP_Data.MemberCardRule> mActiveCard = (from p in entity.MemberCardRules.AsEnumerable() where p.IsActive == true orderby int.Parse(p.RangeFrom) ascending select p).ToList();
                            //    foreach (MemberCardRule m in mActiveCard)
                            //    {

                            //        if (rTo != "Above" && m.RangeTo != "Above")
                            //        {
                            //            if (Convert.ToInt32(rFrom) < Convert.ToInt32(m.RangeFrom) && Convert.ToInt32(rTo) < Convert.ToInt32(m.RangeFrom))
                            //            {
                            //                status = "Add";
                            //            }
                            //            else if (Convert.ToInt32(rFrom) > Convert.ToInt32(m.RangeTo))
                            //            {
                            //                status = "Add";
                            //            }
                            //            else
                            //            {
                            //                status = "Error";
                            //                break;
                            //            }
                            //        }
                            //        else if (rTo == "Above" && m.RangeTo != "Above")
                            //        {
                            //            if (Convert.ToInt32(rFrom) > Convert.ToInt32(m.RangeTo))
                            //            {
                            //                status = "Add";
                            //            }
                            //            else
                            //            {
                            //                status = "Error";
                            //                break;
                            //            }
                            //        }
                            //        else if (rTo != "Above" && m.RangeTo == "Above")
                            //        {
                            //            if (Convert.ToInt32(rFrom) < Convert.ToInt32(m.RangeFrom) && Convert.ToInt32(rTo) < Convert.ToInt32(m.RangeFrom))
                            //            {
                            //                status = "Add";
                            //            }
                            //            else
                            //            {
                            //                status = "Error";
                            //                break;
                            //            }
                            //        }
                            //        else
                            //        {
                            //            status = "Error";
                            //            break;
                            //        }
                            //    }

                            //    if (status == "Add")
                            //    {
                            //        mRule.MemberTypeId = mTypeid;
                            //        mRule.RangeFrom = rFrom;
                            //        mRule.RangeTo = rTo;
                            //        mRule.MCDiscount = 0;
                            //        mRule.BDDiscount = 0;
                            //        mRule.IsCalculatePoints = chkCalculatePoint.Checked;
                            //        mRule.IsActive = chkIsActive.Checked;
                            //        mRule.CreatedBy = MemberShip.UserId;
                            //        mRule.CreatedDate = DateTime.Now;
                            //        entity.MemberCardRules.Add(mRule);
                            //        entity.SaveChanges();

                            //        Clear();
                            //        IsSuccessful = true;
                            //    }
                            //    else if (status == "Error")
                            //    {
                            //        IsSuccessful = false;
                            //        MessageBox.Show("This amount you enter is already used in other member type!");
                            //    }

                        }
                        if (IsSuccessful == true)
                        {
                            MessageBox.Show("Successfully Added!");
                        }
                        Bind_BrandForPromotion();
                        Bind_MemberType();
                        Bind_BrandForReferralProgram();
                        reloadMember();
                        reloadMemberList();
                        reloadEndMemberCardRuleList();
                        backoffice();
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to add new Member Card Rule", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
                //edit rule
                else
                {
                    if (controller.MemberRule.EditOrDelete || MemberShip.isAdmin)
                    {
                        bool isSuccess = false;
                        APP_Data.MemberCardRule EditMemberRule = entity.MemberCardRules.Where(x => x.Id == ruleID).FirstOrDefault();
                        string mType = cboMemberRule.Text;
                        int mTypeid = (from m in entity.MemberTypes where m.Name == mType select m.Id).SingleOrDefault();
                        string rFrom = txtFrom.Text.Trim();
                        string rTo = null;
                        if (rbtBetween.Checked)
                        {
                            rTo = txtTo.Text.Trim();
                        }
                        else
                        {
                            rTo = "Above";
                        }


                        #region OLDconditioncode
                        //int count = entity.MemberCardRules.Count();
                        //if (count == 1)
                        //{
                        //    EditMemberRule.MemberTypeId = mTypeid;
                        //    EditMemberRule.RangeFrom = rFrom;
                        //    EditMemberRule.RangeTo = rTo;
                        //    EditMemberRule.MCDiscount = mDis;
                        //    EditMemberRule.BDDiscount = bDis;
                        //    EditMemberRule.IsCalculatePoints = chkCalculatePoint.Checked;
                        //    EditMemberRule.IsActive = chkIsActive.Checked;
                        //    entity.SaveChanges();
                        //    reloadMember();
                        //    reloadMemberList();
                        //    Clear();
                        //    isSuccess = true;
                        //}
                        //else
                        //{
                        //    if (EditMemberRule.RangeFrom == rFrom && EditMemberRule.RangeTo == rTo)
                        //    {
                        //        EditMemberRule.MemberTypeId = mTypeid;
                        //        EditMemberRule.RangeFrom = rFrom;
                        //        EditMemberRule.RangeTo = rTo;
                        //        EditMemberRule.MCDiscount = mDis;
                        //        EditMemberRule.BDDiscount = bDis;
                        //        EditMemberRule.IsCalculatePoints = chkCalculatePoint.Checked;
                        //        EditMemberRule.IsActive = chkIsActive.Checked;
                        //        entity.SaveChanges();
                        //        reloadMember();
                        //        reloadMemberList();
                        //        Clear();
                        //        isSuccess = true;
                        //    }
                        //    else
                        //    {
                        //        int minRF = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true select Convert.ToInt32(m.RangeFrom)).Min();
                        //        string miniRF = minRF.ToString();
                        //        int minID = (from m in entity.MemberCardRules where m.IsActive == true && m.RangeFrom == miniRF select m.Id).SingleOrDefault();
                        //        string minRTbyID = (from m in entity.MemberCardRules where m.IsActive == true && m.Id == minID && m.RangeTo != "Above" select m.RangeTo).SingleOrDefault();
                        //        int maxiRT = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true && m.RangeTo != "Above" select Convert.ToInt32(m.RangeTo)).Max();
                        //        string maxRT = (from m in entity.MemberCardRules where m.IsActive == true select m.RangeTo).Max();
                        //        if (maxRT != "Above")
                        //        {
                        //            maxRT = maxiRT.ToString();
                        //        }
                        //        int maxRF = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true select Convert.ToInt32(m.RangeFrom)).Max();
                        //        if (rbtBetween.Checked)
                        //        {
                        //            if (Convert.ToInt32(rTo) < minRF)
                        //            {
                        //                EditMemberRule.MemberTypeId = mTypeid;
                        //                EditMemberRule.RangeFrom = rFrom;
                        //                EditMemberRule.RangeTo = rTo;
                        //                EditMemberRule.MCDiscount = mDis;
                        //                EditMemberRule.BDDiscount = bDis;
                        //                EditMemberRule.IsCalculatePoints = chkCalculatePoint.Checked;
                        //                EditMemberRule.IsActive = chkIsActive.Checked;
                        //                entity.SaveChanges();
                        //                reloadMember();
                        //                reloadMemberList();
                        //                Clear();
                        //                isSuccess = true;
                        //            }
                        //            else if (minID == ruleID && Convert.ToInt32(rTo) <= Convert.ToInt32(minRTbyID))
                        //            {
                        //                EditMemberRule.MemberTypeId = mTypeid;
                        //                EditMemberRule.RangeFrom = rFrom;
                        //                EditMemberRule.RangeTo = rTo;
                        //                EditMemberRule.MCDiscount = mDis;
                        //                EditMemberRule.BDDiscount = bDis;
                        //                EditMemberRule.IsCalculatePoints = chkCalculatePoint.Checked;
                        //                EditMemberRule.IsActive = chkIsActive.Checked;
                        //                entity.SaveChanges();
                        //                reloadMember();
                        //                reloadMemberList();
                        //                Clear();
                        //                isSuccess = true;
                        //            }
                        //            else if (Convert.ToInt32(rFrom) >= minRF && maxRF >= Convert.ToInt32(rTo))
                        //            {
                        //                List<APP_Data.MemberCardRule> cardList = new List<APP_Data.MemberCardRule>();
                        //                bool isCorrect = false;
                        //                cardList = (from m in entity.MemberCardRules where m.IsActive == true && m.Id != ruleID orderby m.RangeTo ascending select m).ToList();
                        //                foreach (MemberCardRule mcr in cardList)
                        //                {
                        //                    if (mcr.RangeTo != "Above")
                        //                    {
                        //                        string minRange = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true && Convert.ToInt32(m.RangeFrom) > Convert.ToInt32(mcr.RangeTo) select m.RangeFrom).Min();
                        //                        if (Convert.ToInt32(mcr.RangeTo) < Convert.ToInt32(rFrom) && Convert.ToInt32(minRange) > Convert.ToInt32(rTo))
                        //                        {
                        //                            EditMemberRule.MemberTypeId = mTypeid;
                        //                            EditMemberRule.RangeFrom = rFrom;
                        //                            EditMemberRule.RangeTo = rTo;
                        //                            EditMemberRule.MCDiscount = mDis;
                        //                            EditMemberRule.BDDiscount = bDis;
                        //                            EditMemberRule.IsCalculatePoints = chkCalculatePoint.Checked;
                        //                            EditMemberRule.IsActive = chkIsActive.Checked;
                        //                            entity.SaveChanges();
                        //                            reloadMember();
                        //                            reloadMemberList();
                        //                            Clear();
                        //                            isSuccess = true;
                        //                            isCorrect = false;
                        //                            break;
                        //                        }
                        //                        else
                        //                        {
                        //                            isCorrect = true;
                        //                        }
                        //                    }
                        //                }
                        //                if (isCorrect == true)
                        //                {
                        //                    MessageBox.Show("Please check your Amount!");
                        //                }
                        //            }
                        //            else
                        //            {
                        //                if (maxRT == "Above" && Convert.ToInt32(rFrom) > maxRF)
                        //                {
                        //                    MessageBox.Show("Please check your Amount.You have already entered amount \"" + maxRF.ToString() + " - " + maxRT + "\" in one of your member type!");
                        //                }
                        //                else
                        //                {
                        //                    MessageBox.Show("Please check your Amount!");
                        //                }
                        //            }
                        //        }
                        //        else
                        //        {
                        //            int AboveId = 0;
                        //            if (maxRT == "Above")
                        //            {
                        //                AboveId = (from m in entity.MemberCardRules where m.IsActive == true && m.RangeTo == "Above" select m.Id).SingleOrDefault();
                        //            }
                        //            if (AboveId != 0)
                        //            {
                        //                if (maxRT == "Above" && EditMemberRule.Id != AboveId)
                        //                {
                        //                    MessageBox.Show("Please check your Amount.You have already entered amount \"" + maxRF.ToString() + " - " + maxRT + "\" in one of your member type!");
                        //                }
                        //                else if (maxRT == "Above" && EditMemberRule.Id == AboveId)
                        //                {
                        //                    string maxRTo = (from m in entity.MemberCardRules where m.IsActive == true && m.RangeTo != "Above" select m.RangeTo).Max();
                        //                    if (Convert.ToInt32(maxRTo) < Convert.ToInt32(rFrom))
                        //                    {
                        //                        EditMemberRule.MemberTypeId = mTypeid;
                        //                        EditMemberRule.RangeFrom = rFrom;
                        //                        EditMemberRule.RangeTo = rTo;
                        //                        EditMemberRule.MCDiscount = mDis;
                        //                        EditMemberRule.BDDiscount = bDis;
                        //                        EditMemberRule.IsCalculatePoints = chkCalculatePoint.Checked;
                        //                        EditMemberRule.IsActive = chkIsActive.Checked;
                        //                        entity.SaveChanges();
                        //                        reloadMember();
                        //                        reloadMemberList();
                        //                        Clear();
                        //                        isSuccess = true;
                        //                    }
                        //                    else
                        //                    {
                        //                        MessageBox.Show("Please check your Amount!");
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    if (Convert.ToInt32(rFrom) > Convert.ToInt32(maxRT))
                        //                    {
                        //                        EditMemberRule.MemberTypeId = mTypeid;
                        //                        EditMemberRule.RangeFrom = rFrom;
                        //                        EditMemberRule.RangeTo = rTo;
                        //                        EditMemberRule.MCDiscount = mDis;
                        //                        EditMemberRule.BDDiscount = bDis;
                        //                        EditMemberRule.IsCalculatePoints = chkCalculatePoint.Checked;
                        //                        EditMemberRule.IsActive = chkIsActive.Checked;
                        //                        entity.SaveChanges();
                        //                        reloadMember();
                        //                        reloadMemberList();
                        //                        Clear();
                        //                        isSuccess = true;
                        //                    }
                        //                    else
                        //                    {
                        //                        MessageBox.Show("Please check your Amount!");
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        #endregion
                        if (EditMemberRule.IsActive == false)
                        {
                            if (chkIsActive.Checked == true)
                            {
                                int count = entity.MemberCardRules.Where(x => x.IsActive == true).Count();
                                if (count == 0)
                                {
                                    EditMemberRule.MemberTypeId = mTypeid;
                                    EditMemberRule.RangeFrom = rFrom;
                                    EditMemberRule.RangeTo = rTo;
                                    EditMemberRule.MCDiscount = 0;
                                    EditMemberRule.BDDiscount = 0;
                                    EditMemberRule.IsCalculatePoints = chkCalculatePoint.Checked;
                                    EditMemberRule.IsActive = chkIsActive.Checked;
                                    EditMemberRule.EndBy = MemberShip.UserId;
                                    EditMemberRule.EndDate = DateTime.Now;
                                    entity.SaveChanges();

                                    Clear();
                                    isSuccess = true;
                                }
                                else
                                {

                                    int minRF = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true select Convert.ToInt32(m.RangeFrom)).Min();
                                    int minRT = 0;
                                    int countRange = (from m in entity.MemberCardRules where m.IsActive == true && m.RangeTo != "Above" select m).Count();
                                    if (countRange != 0)
                                    {
                                        minRT = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true && m.RangeTo != "Above" select Convert.ToInt32(m.RangeTo)).Min();
                                    }
                                    else
                                    {
                                        string miniRT = (from m in entity.MemberCardRules where m.IsActive == true select m.RangeTo).Min();
                                    }

                                    string status = "";
                                    List<APP_Data.MemberCardRule> mActiveCard = (from p in entity.MemberCardRules.AsEnumerable() where p.IsActive == true orderby int.Parse(p.RangeFrom) ascending select p).ToList();
                                    foreach (MemberCardRule m in mActiveCard)
                                    {

                                        if (rTo != "Above" && m.RangeTo != "Above")
                                        {
                                            if (Convert.ToInt32(rFrom) < Convert.ToInt32(m.RangeFrom) && Convert.ToInt32(rTo) < Convert.ToInt32(m.RangeFrom))
                                            {
                                                status = "Add";
                                            }
                                            else if (Convert.ToInt32(rFrom) > Convert.ToInt32(m.RangeTo))
                                            {
                                                status = "Add";
                                            }
                                            else
                                            {
                                                status = "Error";
                                                break;
                                            }
                                        }
                                        else if (rTo == "Above" && m.RangeTo != "Above")
                                        {
                                            if (Convert.ToInt32(rFrom) > Convert.ToInt32(m.RangeTo))
                                            {
                                                status = "Add";
                                            }
                                            else
                                            {
                                                status = "Error";
                                                break;
                                            }
                                        }
                                        else if (rTo != "Above" && m.RangeTo == "Above")
                                        {
                                            if (Convert.ToInt32(rFrom) < Convert.ToInt32(m.RangeFrom) && Convert.ToInt32(rTo) < Convert.ToInt32(m.RangeFrom))
                                            {
                                                status = "Add";
                                            }
                                            else
                                            {
                                                status = "Error";
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            status = "Error";
                                            break;
                                        }
                                    }

                                    if (status == "Add")
                                    {
                                        EditMemberRule.MemberTypeId = mTypeid;
                                        EditMemberRule.RangeFrom = rFrom;
                                        EditMemberRule.RangeTo = rTo;
                                        EditMemberRule.MCDiscount = 0;
                                        EditMemberRule.BDDiscount = 0;
                                        EditMemberRule.IsCalculatePoints = chkCalculatePoint.Checked;
                                        EditMemberRule.IsActive = chkIsActive.Checked;
                                        EditMemberRule.EndBy = MemberShip.UserId;
                                        EditMemberRule.EndDate = DateTime.Now;
                                        entity.SaveChanges();

                                        Clear();
                                        isSuccess = true;
                                    }
                                    else if (status == "Error")
                                    {
                                        isSuccess = false;
                                        MessageBox.Show("This Rule can't use.Please Check Balance");
                                    }

                                }
                            }
                            else if (chkIsActive.Checked == false)
                            {
                                EditMemberRule.MemberTypeId = mTypeid;
                                EditMemberRule.RangeFrom = rFrom;
                                EditMemberRule.RangeTo = rTo;
                                EditMemberRule.MCDiscount = 0;
                                EditMemberRule.BDDiscount = 0;
                                EditMemberRule.IsCalculatePoints = chkCalculatePoint.Checked;
                                EditMemberRule.IsActive = chkIsActive.Checked;
                                EditMemberRule.EndBy = MemberShip.UserId;
                                EditMemberRule.EndDate = DateTime.Now;
                                entity.SaveChanges();

                                Clear();
                                isSuccess = true;
                            }
                        }
                        else if (EditMemberRule.IsActive == true)
                        {
                            EditMemberRule.MemberTypeId = mTypeid;
                            EditMemberRule.RangeFrom = rFrom;
                            EditMemberRule.RangeTo = rTo;
                            EditMemberRule.MCDiscount = 0;
                            EditMemberRule.BDDiscount = 0;
                            EditMemberRule.IsCalculatePoints = chkCalculatePoint.Checked;
                            EditMemberRule.IsActive = chkIsActive.Checked;
                            EditMemberRule.EndBy = MemberShip.UserId;
                            EditMemberRule.EndDate = DateTime.Now;
                            entity.SaveChanges();

                            Clear();
                            isSuccess = true;
                        }

                        if (isSuccess == true)
                        {
                            MessageBox.Show("Successfully Updated!");

                        }
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to edit member", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    Bind_BrandForPromotion();
                    Bind_MemberType();
                    Bind_BrandForReferralProgram();
                    reloadMember();
                    reloadMemberList();
                    reloadEndMemberCardRuleList();
                    backoffice();
                }
            }
        }

        private void backoffice()
        {
            bool notbackoffice = Utility.IsNotBackOffice();
            if (notbackoffice)
            {
                Utility.Gpvisible(groupBox1, false);

                btnAddMember.Visible = false;
                // By SYM
                Utility.Gpvisible(gbPoint, false);
                Utility.Gpvisible(gbReferral, false);
                groupBox1.Visible = false;
                gbPoint.Visible = false;
                gbReferral.Visible = false;
                groupBox2.Location = groupBox1.Location;
                groupBox4.Location = gbPoint.Location;
                groupBox6.Location = gbReferral.Location;
                this.groupBox2.Size = new Size(855, 550);
                this.groupBox4.Size = new Size(855, 550);
                this.groupBox6.Size = new Size(855, 550);
                this.tabControl2.Size = new Size(810, 520);
                dgvMemberList.Size = new System.Drawing.Size(803, 500);
                dgvForPromotion.Size = new System.Drawing.Size(803, 500);
                dgvForReferral.Size = new System.Drawing.Size(803, 500);
                dgvOldMemberCardRule.Size = new System.Drawing.Size(803, 500);
                dgvMemberList.Columns[9].Visible = false;
                dgvMemberList.Columns[10].Visible = false;
                dgvOldMemberCardRule.Columns[9].Visible = false;
                dgvOldMemberCardRule.Columns[10].Visible = false;
                dgvForPromotion.Columns[5].Visible = false;
                dgvForPromotion.Columns[6].Visible = false;
                dgvForReferral.Columns[4].Visible = false;
                dgvForReferral.Columns[5].Visible = false;
               // btnNewBrand.Visible = false;
               // btnNewMemeberType.Visible = false; // must be always visible = false cause adding new member type is no longer allowed
               // btnNewBrandForReferral.Visible = false;
            }
            else
            {
                Utility.Gpvisible(gbPoint, true);
                Utility.Gpvisible(gbReferral, true);
                Utility.Gpvisible(groupBox1, true);
                groupBox1.Visible = true;
                gbPoint.Visible = true;
                gbReferral.Visible = true;

                //  btnNewMemeberType.Visible = true; // must be always visible = false cause adding new member type is no longer allowed

               // btnNewBrandForReferral.Visible = true;
            }
        }
        private void MemberRule_Load(object sender, EventArgs e)
        {

            backoffice();

            reloadMemberList();
            reloadEndMemberCardRuleList();
            reloadMember();
            checkRadio();

            // By SYM // for Promotion
            Bind_dgvForPromotion();


            txtBdDiscount.Text = SettingController.birthday_discount.ToString();
            // By SYM // for Referral Program
            Bind_dgvForReferral();
            Bind_BrandForPromotion();
            Bind_MemberType();
            Bind_BrandForReferralProgram();
            Clear();
            AddButton_Control();
            var currentset = entity.Settings.Where(x => x.Key == "IsAdminShop" && x.Value == "1").FirstOrDefault();
            if (currentset == null)
            {
                btnAdd.Visible = false;
                btnAddMember.Visible = false;
                btnNewMemeberType.Visible = false;
                btnAddPromotion.Visible = false;
            }

        }

        //protected void BDDiscountGrid()
        //{
        //    var discount = entity.Settings.Where().ToList();
        //    dgvBdDiscount.DataSource = discount;
        //}
        private void rbtGreater_CheckedChanged(object sender, EventArgs e)
        {
            checkRadio();
        }

        private void rbtBetween_CheckedChanged(object sender, EventArgs e)
        {
            checkRadio();
        }

        private void MemberRule_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(cboMemberRule);
            tp.Hide(txtTo);
            tp.Hide(txtFrom);

            tp.Hide(chkCalculatePoint);
        }

        private void dgvMemberList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int currentId;

            if (e.RowIndex >= 0)
            {
                //Delete
                if (e.ColumnIndex == 10)
                {
                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.MemberRule.EditOrDelete || MemberShip.isAdmin)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            DataGridViewRow row = dgvMemberList.Rows[e.RowIndex];
                            currentId = Convert.ToInt32(row.Cells[0].Value);
                            int getID = 0;
                            getID = (from m in entity.MemberCardRules where m.IsActive == true where m.Id == currentId select m.MemberTypeId).SingleOrDefault();
                            int countID = 0;
                            countID = (from m in entity.Customers where m.MemberTypeID == getID select m).Count();
                            if (countID < 1)
                            {
                                dgvMemberList.DataSource = "";
                                APP_Data.MemberCardRule mType = (from b in entity.MemberCardRules where b.IsActive == true && b.Id == currentId select b).FirstOrDefault();
                                entity.MemberCardRules.Remove(mType);
                                entity.SaveChanges();
                                reloadMemberList();
                                reloadMember();
                                MessageBox.Show("Successfully Deleted!", "Delete Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Cannot Delete Member Type.It has already used in Customer!", "Delete Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to delete member card rule", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                //Edit
                else if (e.ColumnIndex == 9)
                {
                    bool notbackoffice = Utility.IsNotBackOffice();

                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.MemberRule.EditOrDelete || MemberShip.isAdmin)
                    {
                        DataGridViewRow row = dgvMemberList.Rows[e.RowIndex];
                        currentId = Convert.ToInt32(row.Cells[0].Value);

                        APP_Data.MemberCardRule mType = (from b in entity.MemberCardRules where b.IsActive == true && b.Id == currentId select b).FirstOrDefault();
                        APP_Data.MemberType meType = (from m in entity.MemberTypes where m.Id == mType.MemberTypeId select m).FirstOrDefault();
                        cboMemberRule.Text = meType.Name;
                        txtFrom.Text = mType.RangeFrom;
                        oldRangeFromAmount = Convert.ToInt32(mType.RangeFrom);
                        txtTo.Text = mType.RangeTo;
                        if (mType.RangeTo.ToString() == "Above")
                        {
                            rbtGreater.Checked = true;
                        }
                        else
                        {
                            rbtBetween.Checked = true;
                        }

                        chkCalculatePoint.Checked = Convert.ToBoolean(mType.IsCalculatePoints);
                        chkIsActive.Checked = Convert.ToBoolean(mType.IsActive);

                        groupBox1.Text = "Edit Member Card Rule";
                        rbtGreater.Enabled = false;
                        rbtBetween.Enabled = false;
                        txtFrom.Enabled = false;
                        txtTo.Enabled = false;
                        isEdit = true;

                        backoffice();
                        ruleID = mType.Id;
                        btnAdd.Image = Properties.Resources.update_small;
                        reloadMember(meType.Name);

                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to edit member card rule", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void dgvOldMemberCardRule_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int currentId;

            if (e.RowIndex >= 0)
            {

                if (e.ColumnIndex == 9)
                {
                    bool notbackoffice = Utility.IsNotBackOffice();

                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.MemberRule.EditOrDelete || MemberShip.isAdmin)
                    {
                        DataGridViewRow row = dgvOldMemberCardRule.Rows[e.RowIndex];
                        currentId = Convert.ToInt32(row.Cells[0].Value);

                        APP_Data.MemberCardRule mType = (from b in entity.MemberCardRules where b.Id == currentId select b).FirstOrDefault();
                        APP_Data.MemberType meType = (from m in entity.MemberTypes where m.Id == mType.MemberTypeId select m).FirstOrDefault();
                        cboMemberRule.Text = meType.Name;
                        txtFrom.Text = mType.RangeFrom;
                        oldRangeFromAmount = Convert.ToInt32(mType.RangeFrom);
                        txtTo.Text = mType.RangeTo;
                        if (mType.RangeTo.ToString() == "Above")
                        {
                            rbtGreater.Checked = true;
                        }
                        else
                        {
                            rbtBetween.Checked = true;
                        }

                        chkCalculatePoint.Checked = Convert.ToBoolean(mType.IsCalculatePoints);
                        chkIsActive.Checked = Convert.ToBoolean(mType.IsActive);

                        groupBox1.Text = "Edit Member Card Rule";
                        rbtGreater.Enabled = false;
                        rbtBetween.Enabled = false;
                        txtFrom.Enabled = false;
                        txtTo.Enabled = false;
                        isEdit = true;

                        backoffice();
                        ruleID = mType.Id;
                        btnAdd.Image = Properties.Resources.update_small;
                        reloadMember(meType.Name);

                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to edit member card rule", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void dgvMemberList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvMemberList.Rows)
            {
                APP_Data.MemberCardRule cRule = (APP_Data.MemberCardRule)row.DataBoundItem;
                string amt = cRule.RangeFrom + " - " + cRule.RangeTo;
                row.Cells[0].Value = (object)cRule.Id;
                row.Cells[1].Value = (object)cRule.MemberType.Name;
                row.Cells[2].Value = (object)amt;
                row.Cells[7].Value = cRule.CreatedDate.ToString();
                row.Cells[8].Value = (from p in entity.Users where p.Id == cRule.CreatedBy select p.Name).FirstOrDefault();
            }
        }



        private void txtMemberDis_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtBirthdayDis_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (rbtBetween.Checked)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void cboMemberRule_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(cboMemberRule);
        }

        private void txtFrom_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtFrom);
        }

        private void txtTo_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtTo);
        }



        // By SYM
        private void btnNewMemeberType_Click(object sender, EventArgs e)
        {
            newMemberType newType = new newMemberType();
            newType.ShowDialog();
        }
        // By SYM  
        private void btnAddPromotion_Click(object sender, EventArgs e)
        {
            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            Boolean HaveError = false;
            if (cboBrandForPromotion.Text == "Select")
            {
                tp.SetToolTip(cboBrandForPromotion, "Error");
                tp.Show("Please select Brand!", cboBrandForPromotion);
                HaveError = true;
            }
            if (cboMemberType.Text == "Select")
            {
                tp.SetToolTip(cboMemberType, "Error");
                tp.Show("Please select Member Type!", cboMemberType);
                HaveError = true;
            }
            else if (txtPointAmount.Text.Trim() == string.Empty || txtPointAmount.Text.Trim() == "0")
            {
                tp.SetToolTip(txtPointAmount, "Error");
                tp.Show("Please fill up Amount!", txtPointAmount);
                HaveError = true;
            }
            else if (txtPoint.Text.Trim() == string.Empty || txtPoint.Text.Trim() == "0")
            {
                tp.SetToolTip(txtPoint, "Error");
                tp.Show("Please fill up point!", txtPoint);
                HaveError = true;
            }

            if (!HaveError)
            {
                if (currentId == 0)
                {
                    APP_Data.Promotion promotionObj = new Promotion();
                    promotionObj.MemberTypeId = Convert.ToInt32(cboMemberType.SelectedValue);
                    promotionObj.Amount = Convert.ToInt32(txtPointAmount.Text);
                    promotionObj.Point = txtPoint.Text;
                    promotionObj.BrandId = Convert.ToInt32(cboBrandForPromotion.SelectedValue);
                    entity.Promotions.Add(promotionObj);


                    APP_Data.Point_History pointHistory = new Point_History();
                    pointHistory.BrandId = Convert.ToInt32(cboBrandForPromotion.SelectedValue);
                    pointHistory.CreatedBy = MemberShip.UserId;
                    pointHistory.CreatedDate = DateTime.Now;
                    pointHistory.PRMemberTypeId = Convert.ToInt32(cboMemberType.SelectedValue);
                    pointHistory.PRPointAmount = Convert.ToDecimal(txtPointAmount.Text);
                    pointHistory.Point = Convert.ToDecimal(txtPoint.Text);
                    pointHistory.Status = "Promotion";
                    entity.Point_History.Add(pointHistory);

                    entity.SaveChanges();
                    MessageBox.Show("Successfully Saved!", "Save");
                    Clear();
                    Bind_MemberType();
                    Bind_BrandForPromotion();
                    Bind_dgvForPromotion();
                    CancelPromotion();
                }
                else
                {
                    //string name = cboMemberType.Text;

                    Promotion _promotion = (from p in entity.Promotions where p.Id == currentId select p).FirstOrDefault();
                    MemberType _memberType = (from m in entity.MemberTypes where m.Name == cboMemberType.Text select m).FirstOrDefault();
                    APP_Data.Brand _brand = (from b in entity.Brands where b.Name == cboBrandForPromotion.Text select b).FirstOrDefault();
                    _promotion.MemberTypeId = Convert.ToInt32(_memberType.Id);

                    _promotion.Amount = Convert.ToInt32(txtPointAmount.Text);
                    _promotion.Point = txtPoint.Text.Trim();
                    _promotion.BrandId = Convert.ToInt32(_brand.Id);
                    entity.Entry(_promotion).State = EntityState.Modified;


                    APP_Data.Point_History pointHistory = new Point_History();
                    pointHistory.BrandId = Convert.ToInt32(_brand.Id);
                    pointHistory.CreatedBy = MemberShip.UserId;
                    pointHistory.CreatedDate = DateTime.Now;
                    pointHistory.PRMemberTypeId = Convert.ToInt32(_memberType.Id);
                    pointHistory.PRPointAmount = Convert.ToDecimal(txtPointAmount.Text.Trim());
                    pointHistory.Point = Convert.ToDecimal(txtPoint.Text.Trim());
                    pointHistory.Status = "Promotion";
                    entity.Point_History.Add(pointHistory);

                    entity.SaveChanges();
                    MessageBox.Show("Successfully Updated!", "Update");
                    Clear();
                    Bind_MemberType();
                    Bind_BrandForPromotion();
                    Bind_dgvForPromotion();
                    CancelPromotion();
                    btnAddPromotion.Image = Properties.Resources.add_small;
                }

            }
        }
        // By SYM
        private void btnCancelPromotion_Click(object sender, EventArgs e)
        {
            CancelPromotion();
        }
        // By SYM
        private void dgvForPromotion_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow rows in dgvForPromotion.Rows)
            {
                APP_Data.Promotion promotion = (APP_Data.Promotion)rows.DataBoundItem;
                rows.Cells[0].Value = (object)promotion.Id;
                rows.Cells[1].Value = (object)promotion.Brand.Name;
                rows.Cells[2].Value = (object)promotion.MemberType.Name;
            }
        }
        // By SYM
        private void dgvForPromotion_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                //Delete
                if (e.ColumnIndex == 6)
                {
                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.MemberRule.EditOrDelete || MemberShip.isAdmin)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            DataGridViewRow row = dgvForPromotion.Rows[e.RowIndex];
                            currentId = Convert.ToInt32(row.Cells[0].Value);
                            //int getID = 0;
                            //getID = (from p in entity.Promotions where p.Id == currentId select p.MemberTypeId).SingleOrDefault();
                            //int countID = 0;
                            //countID = (from m in entity.Customers where m.MemberTypeID == getID select m).Count();
                            //if (countID < 1)
                            //{
                            dgvForPromotion.DataSource = "";
                            APP_Data.Promotion promotion = (from p in entity.Promotions where p.Id == currentId select p).FirstOrDefault();
                            entity.Promotions.Remove(promotion);
                            entity.SaveChanges();

                            MessageBox.Show("Successfully Deleted!", "Delete Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Bind_dgvForPromotion();
                            Bind_MemberType();
                            Bind_BrandForPromotion();
                            CancelPromotion();
                            //}
                            //else
                            //{
                            //    MessageBox.Show("Cannot Delete Member Type.It has already used in Customer!", "Delete Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //}
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to delete Promotion", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                //Edit
                else if (e.ColumnIndex == 5)
                {
                    bool notbackoffice = Utility.IsNotBackOffice();

                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.MemberRule.EditOrDelete || MemberShip.isAdmin)
                    {
                        DataGridViewRow row = dgvForPromotion.Rows[e.RowIndex];
                        currentId = Convert.ToInt32(row.Cells[0].Value);

                        APP_Data.Promotion promotion = (from p in entity.Promotions where p.Id == currentId select p).FirstOrDefault();
                        int brandid = Convert.ToInt32(promotion.BrandId);
                        APP_Data.MemberType memberType = (from m in entity.MemberTypes where m.Id == promotion.MemberTypeId select m).FirstOrDefault();
                        APP_Data.Brand brand = (from b in entity.Brands where b.Id == promotion.BrandId select b).FirstOrDefault();
                        cboMemberType.Text = Convert.ToString(memberType.Name);
                        txtPointAmount.Text = Convert.ToString(promotion.Amount);
                        txtPoint.Text = promotion.Point.Trim();
                        cboBrandForPromotion.Text = Convert.ToString(brand.Name);

                        gbPoint.Text = "Edit Point For Special Product Line";

                        isEdit = true;


                        backoffice();

                        ruleID = promotion.Id;
                        btnAddPromotion.Image = Properties.Resources.update_small;

                        // Bind_BrandForPromotion(brand.Name);
                        LoadPromotionMemberCbo(memberType.Name, brandid, isEdit);

                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to edit promotion", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }
        // By SYM
        private void btnNewBrandForReferral_Click(object sender, EventArgs e)
        {
            Brand brand = new Brand();
            brand.ShowDialog();
        }
        // By SYM
        private void cboMemberType_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(cboMemberType);
        }
        // By SYM
        private void btnNewBrand_Click(object sender, EventArgs e)
        {
            Brand brand = new Brand();
            brand.ShowDialog();
        }
        // By SYM
        private void btnAddReferral_Click(object sender, EventArgs e)
        {
            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            Boolean HaveError = false;
            if (cboBrand.Text == "Select")
            {
                tp.SetToolTip(cboBrand, "Error");
                tp.Show("Please select Brand!", cboBrand);
                HaveError = true;
            }
            else if (txtMiniAmount.Text.Trim() == string.Empty || txtMiniAmount.Text.Trim() == "0")
            {
                tp.SetToolTip(txtMiniAmount, "Error");
                tp.Show("Please fill up Amount!", txtMiniAmount);
                HaveError = true;
            }
            else if (txtReferralPoint.Text.Trim() == string.Empty || txtReferralPoint.Text.Trim() == "0")
            {
                tp.SetToolTip(txtReferralPoint, "Error");
                tp.Show("Please fill up Point!", txtReferralPoint);
                HaveError = true;
            }

            var _referralInfo = entity.ReferralPrograms.Where(x => x.IsActive == true).FirstOrDefault();

            if (!HaveError)
            {
                if (currentId == 0)
                {

                    APP_Data.ReferralProgram referralProgramObj = new ReferralProgram();
                    referralProgramObj.BrandId = Convert.ToInt32(cboBrand.SelectedValue);
                    referralProgramObj.MiniPurchaseAmount = Convert.ToInt32(txtMiniAmount.Text.Trim());
                    referralProgramObj.ReferralPoint = Convert.ToDecimal(txtReferralPoint.Text);
                    referralProgramObj.IsActive = true;
                    entity.ReferralPrograms.Add(referralProgramObj);

                    APP_Data.Point_History PointHistory = new Point_History();
                    PointHistory.BrandId = Convert.ToInt32(cboBrand.SelectedValue);
                    PointHistory.CreatedBy = MemberShip.UserId;
                    PointHistory.CreatedDate = DateTime.Now;
                    PointHistory.REFMiniPurchaseAmount = Convert.ToDecimal(txtMiniAmount.Text.Trim());
                    PointHistory.Point = Convert.ToDecimal(txtReferralPoint.Text.Trim());
                    PointHistory.Status = "ReferralProgram";
                    entity.Point_History.Add(PointHistory);

                    entity.SaveChanges();

                    MessageBox.Show("Successfully Saved!", "Save");
                    Clear();
                    Bind_BrandForReferralProgram();
                    Bind_dgvForReferral();
                    CancelReferralProgram();
                }
                else
                {
                    //string name = cboMemberType.Text;

                    ReferralProgram _referralProgram = (from r in entity.ReferralPrograms where r.Id == currentId select r).FirstOrDefault();
                    APP_Data.Brand _brand = (from b in entity.Brands where b.Name == cboBrand.Text select b).FirstOrDefault();
                    _referralProgram.BrandId = Convert.ToInt32(_brand.Id);
                    _referralProgram.MiniPurchaseAmount = Convert.ToInt32(txtMiniAmount.Text.Trim());
                    _referralProgram.ReferralPoint = Convert.ToDecimal(txtReferralPoint.Text.Trim());
                    _referralProgram.IsActive = true;
                    entity.Entry(_referralProgram).State = EntityState.Modified;


                    APP_Data.Point_History PointHistory = new Point_History();
                    PointHistory.BrandId = Convert.ToInt32(_brand.Id);
                    PointHistory.CreatedBy = MemberShip.UserId;
                    PointHistory.CreatedDate = DateTime.Now;
                    PointHistory.REFMiniPurchaseAmount = Convert.ToDecimal(txtMiniAmount.Text.Trim());
                    PointHistory.Point = Convert.ToDecimal(txtReferralPoint.Text.Trim());
                    PointHistory.Status = "ReferralProgram";
                    entity.Point_History.Add(PointHistory);

                    entity.SaveChanges();

                    MessageBox.Show("Successfully Updated!", "Update");
                    Clear();
                    Bind_BrandForReferralProgram();
                    Bind_dgvForReferral();
                    CancelReferralProgram();
                    btnAddReferral.Image = Properties.Resources.add_small;
                }
                AddButton_Control();

            }
        }
        // By SYM
        private void btnCancelReferral_Click(object sender, EventArgs e)
        {
            CancelReferralProgram();
            AddButton_Control();
        }
        // By SYM
        private void dgvForReferral_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //Delete
                if (e.ColumnIndex == 5)
                {
                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.MemberRule.EditOrDelete || MemberShip.isAdmin)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            btnCancelReferral.PerformClick();
                            DataGridViewRow row = dgvForReferral.Rows[e.RowIndex];
                            currentId = Convert.ToInt32(row.Cells[0].Value);
                            //int getID = 0;
                            //getID = (from r in entity.ReferralPrograms where r.Id == currentId select r.BrandId).SingleOrDefault();
                            //int countID = 0;
                            //countID = (from m in entity.Customers where m.MemberTypeID == getID select m).Count();
                            //if (countID < 1)
                            //{
                            dgvForReferral.DataSource = "";
                            APP_Data.ReferralProgram referralProgram = (from r in entity.ReferralPrograms where r.Id == currentId select r).FirstOrDefault();
                            entity.ReferralPrograms.Remove(referralProgram);

                            entity.SaveChanges();

                            MessageBox.Show("Successfully Deleted!", "Delete Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Bind_dgvForReferral();
                            Bind_BrandForReferralProgram();
                            CancelReferralProgram();
                            //}
                            //else
                            //{
                            //    MessageBox.Show("Cannot Delete Referral Program.It has already used in Customer!", "Delete Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //}
                            AddButton_Control();
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to delete Referral Program", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                //Edit
                else if (e.ColumnIndex == 4)
                {
                    bool notbackoffice = Utility.IsNotBackOffice();

                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.MemberRule.EditOrDelete || MemberShip.isAdmin)
                    {
                        DataGridViewRow row = dgvForReferral.Rows[e.RowIndex];
                        currentId = Convert.ToInt32(row.Cells[0].Value);

                        APP_Data.ReferralProgram referralProgram = (from r in entity.ReferralPrograms where r.Id == currentId select r).FirstOrDefault();
                        APP_Data.Brand brand = (from b in entity.Brands where b.Id == referralProgram.BrandId select b).FirstOrDefault();
                        cboBrand.Text = Convert.ToString(brand.Name);
                        txtMiniAmount.Text = Convert.ToString(referralProgram.MiniPurchaseAmount);
                        txtReferralPoint.Text = referralProgram.ReferralPoint.ToString();


                        gbReferral.Text = "Edit Line Referral Point";
                        isEdit = true;


                        backoffice();
                        ruleID = referralProgram.Id;
                        btnAddReferral.Image = Properties.Resources.update_small;
                        Bind_BrandForReferralProgram(brand.Name);
                        btnAddReferral.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to edit Referral Program", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }
        // By SYM
        private void dgvForReferral_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow rows in dgvForReferral.Rows)
            {
                APP_Data.ReferralProgram referralProgram = (APP_Data.ReferralProgram)rows.DataBoundItem;
                rows.Cells[0].Value = (object)referralProgram.Id;
                rows.Cells[1].Value = (object)referralProgram.Brand.Name;

            }
        }

        private void cboBrandPromotion_SelectedIndex(object sender, EventArgs e)
        {
            LoadPromotionMemberCbo();
        }


        #endregion

        #region Method

        private void checkRadio()
        {
            if (rbtGreater.Checked)
            {
                txtTo.Enabled = false;
                //txtTo.Text = "0";
                lblTo.Enabled = false;
            }
            if (rbtBetween.Checked)
            {
                txtTo.Enabled = true;
                if (txtTo.Text.Trim() == "Above") txtTo.Text = "0";
                lblTo.Enabled = true;
            }
        }

        public void reloadMember(string memb = null)
        {
            reloadMemberList();
            List<APP_Data.MemberType> MemList = new List<APP_Data.MemberType>();
            List<APP_Data.MemberType> MemberList = new List<APP_Data.MemberType>();
            int count = 0;
            APP_Data.MemberType memberObj1 = new APP_Data.MemberType();
            memberObj1.Id = 0;
            memberObj1.Name = "Select";
            MemberList.Add(memberObj1);
            MemList = (from p in entity.MemberTypes
                       where p.IsDelete == false
                       select p).ToList();

            MemberList.InsertRange(1, MemList);
            //foreach (MemberType newMem in MemList)
            //{
            //    count = (from m in entity.MemberCardRules where m.MemberTypeId == newMem.Id select m).Count();
            //    if (count == 0)
            //    {
            //        APP_Data.MemberType mType = (from m in entity.MemberTypes where m.Id == newMem.Id select m).FirstOrDefault();
            //        MemberList.Add(mType);
            //    }
            //}
            cboMemberRule.DataSource = null;
            cboMemberRule.DataSource = MemberList;
            cboMemberRule.DisplayMember = "Name";
            cboMemberRule.ValueMember = "Id";
            cboMemberRule.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboMemberRule.AutoCompleteSource = AutoCompleteSource.ListItems;
            if (memb == null)
            {
                cboMemberRule.Text = "Select";
            }
            else
            {
                cboMemberRule.Text = memb;
            }

        }

        // By SYM  
        private void Bind_MemberType(string member = null, int brandId = 0)
        {
            Bind_dgvForPromotion();
            LoadPromotionMemberCbo(member, brandId);
            //entity = new POSEntities();


            //List<APP_Data.MemberType> memberTypeList = new List<APP_Data.MemberType>();
            //APP_Data.MemberType memberTypes = new APP_Data.MemberType();
            //memberTypes.Id = 0;
            //if(member==null)
            //{
            //    memberTypes.Name = "Select";
            //}
            //else
            //{
            //    memberTypes.Name = member;
            //}
            //memberTypeList.Add(memberTypes);
            //memberTypeList.AddRange(entity.MemberTypes.ToList());
            //cboMemberType.DataSource = memberTypeList;
            //cboMemberType.DisplayMember = "Name";
            //cboMemberType.ValueMember = "Id";





        }
        // By SYM  
        public void Bind_BrandForReferralProgram(string brandName = null)
        {
            Bind_dgvForReferral();

            List<APP_Data.Brand> braList = new List<APP_Data.Brand>();
            List<APP_Data.Brand> BrandList = new List<APP_Data.Brand>();
            int count = 0;
            APP_Data.Brand brandObj1 = new APP_Data.Brand();
            brandObj1.Id = 0;
            brandObj1.Name = "Select";
            BrandList.Add(brandObj1);
            braList = entity.Brands.Where(x => x.IsDelete == false).ToList();
            foreach (APP_Data.Brand newBrand in braList)
            {
                count = (from r in entity.ReferralPrograms where r.BrandId == newBrand.Id select r).Count();
                if (count == 0)
                {
                    APP_Data.Brand brand = (from b in entity.Brands where b.Id == newBrand.Id select b).FirstOrDefault();
                    BrandList.Add(brand);
                }
            }
            cboBrand.DataSource = null;
            cboBrand.DataSource = BrandList;
            cboBrand.DisplayMember = "Name";
            cboBrand.ValueMember = "Id";
            cboBrand.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboBrand.AutoCompleteSource = AutoCompleteSource.ListItems;
            if (brandName == null)
            {
                cboBrand.Text = "Select";
            }
            else
            {
                cboBrand.Text = brandName;
            }

        }
        // By SYM  
        public void Bind_BrandForPromotion(string brandName = null)
        {
            Bind_dgvForPromotion();

            //entity = new POSEntities();
            //List<APP_Data.Brand> brandList = new List<APP_Data.Brand>();
            //APP_Data.Brand brands = new APP_Data.Brand();
            //brands.Id = 0;
            //brandList.Add(brands);
            //brandList.AddRange(entity.Brands.ToList());

            List<APP_Data.Brand> braList = new List<APP_Data.Brand>();
            List<APP_Data.Brand> BrandList = new List<APP_Data.Brand>();
            List<int> member = new List<int>();
            int count = 0;
            APP_Data.Brand brandObj1 = new APP_Data.Brand();
            brandObj1.Id = 0;
            brandObj1.Name = "Select";
            BrandList.Add(brandObj1);
            member = (from p in entity.MemberTypes
                      join c in entity.MemberCardRules
                                    on p.Id equals c.MemberTypeId
                      where p.IsDelete == false && c.IsActive == true
                      select p.Id).ToList();
            braList = entity.Brands.Where(x => x.IsDelete == false).ToList();
            foreach (APP_Data.Brand newBrand in braList)
            {

                count = (from r in entity.Promotions where r.BrandId == newBrand.Id && member.Contains(r.MemberTypeId) select r).Count();
                if (count < member.Count())
                {
                    APP_Data.Brand brand = (from b in entity.Brands where b.Id == newBrand.Id select b).FirstOrDefault();
                    BrandList.Add(brand);

                }
            }


            cboBrandForPromotion.DataSource = null;
            cboBrandForPromotion.DataSource = BrandList;
            cboBrandForPromotion.DisplayMember = "Name";
            cboBrandForPromotion.ValueMember = "Id";
            isStart = true;
            cboBrandForPromotion.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboBrandForPromotion.AutoCompleteSource = AutoCompleteSource.ListItems;

            if (brandName == null)
            {
                cboBrandForPromotion.Text = "Select";
            }
            else
            {
                cboBrandForPromotion.Text = brandName;
            }
        }

        // By SYM
        public void Bind_dgvForPromotion()
        {
            entity = new POSEntities();
            dgvForPromotion.AutoGenerateColumns = false;
            dgvForPromotion.DataSource = (from p in entity.Promotions orderby p.BrandId ascending select p).ToList();
        }
        // By SYM
        public void Bind_dgvForReferral()
        {
            entity = new POSEntities();
            dgvForReferral.AutoGenerateColumns = false;
            dgvForReferral.DataSource = (from r in entity.ReferralPrograms where r.IsActive == true orderby r.Id ascending select r).ToList();
        }

        //

        // By SYM
        private void CancelPromotion()
        {
            isEdit = false;
            cboMemberType.SelectedIndex = -1;
            cboBrandForPromotion.SelectedIndex = -1;
            Bind_MemberType();
            Bind_BrandForPromotion();
            currentId = 0;
            txtPointAmount.Text = "10000";
            txtPoint.Text = "0";
            this.Text = "Add New Promotion";
            btnAddPromotion.Image = Properties.Resources.add_small;
            backoffice();
        }
        // By SYM
        private void CancelReferralProgram()
        {
            isEdit = false;
            cboBrand.SelectedIndex = -1;
            Bind_BrandForReferralProgram();
            currentId = 0;
            txtMiniAmount.Text = "0";
            txtReferralPoint.Text = "0";
            this.Text = "Add New Referral Program";
            btnAddReferral.Image = Properties.Resources.add_small;
            backoffice();
        }

        public void reloadMemberList()
        {
            entity = new POSEntities();
            dgvMemberList.AutoGenerateColumns = false;
            dgvMemberList.DataSource = (from m in entity.MemberCardRules.AsEnumerable()
                                        where m.IsActive == true
                                        orderby int.Parse(m.RangeFrom)
                                        select m).ToList();

            //  dgvMemberList.DataSource = entity.MemberCardRules.Where(x=>x.IsActive==true).ToList().OrderBy(x => int.Parse(x.RangeFrom));
        }

        public void reloadEndMemberCardRuleList()
        {
            entity = new POSEntities();
            dgvOldMemberCardRule.AutoGenerateColumns = false;
            dgvOldMemberCardRule.DataSource = (from m in entity.MemberCardRules.AsEnumerable()
                                               where m.IsActive == false
                                               orderby int.Parse(m.RangeFrom)
                                               select m).ToList();
        }


        private void Clear()
        {
            isEdit = false;
            chkCalculatePoint.Checked = false;
            ruleID = 0;
            cboMemberRule.Text = "Select";
            txtTo.Text = "0";
            txtFrom.Text = "0";

            rbtBetween.Checked = true;
            chkIsActive.Checked = false;
            rbtGreater.Enabled = true;
            rbtBetween.Enabled = true;
            txtFrom.Enabled = true;
            txtTo.Enabled = true;
            gbPoint.Text = "Add Point For Special Product Line";
            groupBox1.Text = "Add Member Card Rule";
            gbReferral.Text = "Add Line Referral Point";
            btnAdd.Image = Properties.Resources.add_small;
            backoffice();
        }



        public void LoadPromotionMemberCbo(string member = null, int brandId = 0, bool isedit = false)
        {
            if (isStart == true)
            {
                int brand = Convert.ToInt32(cboBrandForPromotion.SelectedValue);
                List<APP_Data.MemberType> member1 = new List<MemberType>();
                List<APP_Data.MemberType> memberlist = new List<MemberType>();
                member1 = (from p in entity.MemberTypes
                           join c in entity.MemberCardRules
                                         on p.Id equals c.MemberTypeId
                           where p.IsDelete == false && c.IsActive == true
                           select p).ToList();

                if (isedit == true)
                {

                    foreach (MemberType newmember in member1)
                    {
                        int Count = (from p in entity.Promotions where p.BrandId == brandId && p.MemberTypeId == newmember.Id && p.MemberType.Name != member select p).Count();
                        if (Count == 0)
                        {
                            APP_Data.MemberType mem = (from b in entity.MemberTypes where b.Id == newmember.Id select b).FirstOrDefault();
                            memberlist.Add(mem);
                        }
                    }
                    cboMemberType.DataSource = null;
                    cboMemberType.DataSource = memberlist;
                    cboMemberType.DisplayMember = "Name";
                    cboMemberType.ValueMember = "Id";
                }
                else
                {
                    if (brand != 0)
                    {
                        foreach (MemberType newmember in member1)
                        {
                            int Count = (from p in entity.Promotions where p.BrandId == brand && p.MemberTypeId == newmember.Id select p).Count();
                            if (Count == 0)
                            {
                                APP_Data.MemberType mem = (from b in entity.MemberTypes where b.Id == newmember.Id select b).FirstOrDefault();
                                memberlist.Add(mem);
                            }
                        }
                        cboMemberType.DataSource = null;
                        cboMemberType.DataSource = memberlist;
                        cboMemberType.DisplayMember = "Name";
                        cboMemberType.ValueMember = "Id";
                    }
                    else
                    {

                        List<APP_Data.MemberType> memberTypeList = new List<APP_Data.MemberType>();
                        APP_Data.MemberType memberTypes = new APP_Data.MemberType();
                        memberTypes.Id = 0;
                        if (member == null)
                        {
                            memberTypes.Name = "Select";
                        }
                        else
                        {
                            memberTypes.Name = member;
                        }
                        memberTypeList.Add(memberTypes);
                        memberTypeList.AddRange((from p in entity.MemberTypes
                                                 join c in entity.MemberCardRules
                                                               on p.Id equals c.MemberTypeId
                                                 where p.IsDelete == false && c.IsActive == true
                                                 select p).ToList());
                        cboMemberType.DataSource = memberTypeList;
                        cboMemberType.DisplayMember = "Name";
                        cboMemberType.ValueMember = "Id";
                    }
                }
                if (member == null)
                {

                    cboMemberType.Text = "Select";
                }
                else
                {
                    cboMemberType.Text = member;
                }
            }
        }

        private void txtPointAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
             (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.Count() <= 0))
            {
                e.Handled = true;
            }
        }


        private void txtPoint_Keypress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.Count() <= 0))
            {
                e.Handled = true;
            }
        }

        private void txtFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }


        private void txtMiniAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtReferralPoint_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        #endregion

        private void dgvOldMemberCardRule_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvOldMemberCardRule.Rows)
            {
                APP_Data.MemberCardRule cRule = (APP_Data.MemberCardRule)row.DataBoundItem;
                string amt = cRule.RangeFrom + " - " + cRule.RangeTo;
                row.Cells[0].Value = (object)cRule.Id;
                row.Cells[1].Value = (object)cRule.MemberType.Name;
                row.Cells[2].Value = (object)amt;
                row.Cells[7].Value = cRule.EndDate.ToString();
                row.Cells[8].Value = (from p in entity.Users where p.Id == cRule.EndBy select p.Name).FirstOrDefault();
            }
        }

        private bool Get_Referral()
        {
            entity = new POSEntities();
            var _referralInfo = entity.ReferralPrograms.Where(x => x.IsActive == true || x.IsActive == null).FirstOrDefault();
            if (_referralInfo != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void AddButton_Control()
        {
            bool IsEnable = Get_Referral();

            switch (IsEnable)
            {
                case true:
                    btnAddReferral.Enabled = true;
                    break;
                case false:
                    btnAddReferral.Enabled = false;
                    break;
            }
        }

        private void btnBdDiscount_Click(object sender, EventArgs e)
        {
            ToolTip tp = new ToolTip();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            if (string.IsNullOrEmpty(txtBdDiscount.Text))
            {
                tp.SetToolTip(txtBdDiscount, "Fill discount rate");
                tp.Show("Fill discount rate", this.txtBdDiscount);
                return;

            }
            else if (!Regex.IsMatch(txtBdDiscount.Text, @"^\-{0,1}\d+(.\d+){0,1}$"))
            {
                tp.SetToolTip(txtBdDiscount, "");
                tp.Show("Discount rate must be number", this.txtBdDiscount);
                return;
            }
            SettingController.birthday_discount = Convert.ToDecimal(txtBdDiscount.Text);
            timer1.Start();
            timer1.Interval = 3000;
            panelSuccess.Visible = true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            panelSuccess.Visible = false;
            timer1.Stop();
        }
    }
}
