using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using BukuKita.Model;
using static BookLibrary.BookLib;

namespace UnitTest
{
    [TestClass]
    public sealed class AlwinMarbun_Test
    {
        [TestMethod]
        public void Test_CreateApproval()
        {
            // Arrange
            string idApproval = "APV001";
            string idBuku = "B01";
            string judulBuku = "Buku Test";
            string namaPeminjam = "User Test";

            // Act
            var approval = new Approval(idApproval, idBuku, judulBuku, namaPeminjam);

            // Assert
            Assert.AreEqual(idApproval, approval.idApproval);
            Assert.AreEqual(idBuku, approval.idBuku);
            Assert.AreEqual(judulBuku, approval.judulBuku);
            Assert.AreEqual(namaPeminjam, approval.namaPeminjam);
            Assert.AreEqual("Pending", approval.status);
        }

        [TestMethod]
        public void Test_ApproveApproval()
        {
            // Arrange
            var approval = new Approval("APV001", "B01", "Buku Test", "User Test");

            // Act
            approval.status = "Approved";
            approval.keterangan = "Disetujui oleh Admin";

            // Assert
            Assert.AreEqual("Approved", approval.status);
            Assert.AreEqual("Disetujui oleh Admin", approval.keterangan);
        }

        [TestMethod]
        public void Test_RejectApproval()
        {
            // Arrange
            var approval = new Approval("APV001", "B01", "Buku Test", "User Test");

            // Act
            approval.status = "Rejected";
            approval.keterangan = "Buku sedang diperbaiki";

            // Assert
            Assert.AreEqual("Rejected", approval.status);
            Assert.AreEqual("Buku sedang diperbaiki", approval.keterangan);
        }

        [TestMethod]
        public void Test_DisplayApprovalInfo()
        {
            // Arrange
            var approval = new Approval("APV001", "B01", "Buku Test", "User Test");
            approval.keterangan = "Test keterangan";

            var originalOut = Console.Out;
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Act
            approval.DisplayInfo();
            string hasil = stringWriter.ToString();
            Console.SetOut(originalOut);

            // Assert
            Assert.IsTrue(hasil.Contains("ID Approval: APV001"));
            Assert.IsTrue(hasil.Contains("ID Buku: B01"));
            Assert.IsTrue(hasil.Contains("Judul Buku: Buku Test"));
            Assert.IsTrue(hasil.Contains("Nama Peminjam: User Test"));
            Assert.IsTrue(hasil.Contains("Status: Pending"));
            Assert.IsTrue(hasil.Contains("Keterangan: Test keterangan"));
        }

        [TestMethod]
        public void Test_ApprovalDefaultConstructor()
        {
            // Act
            var approval = new Approval();

            // Assert
            Assert.AreEqual("Pending", approval.status);
            Assert.AreEqual("", approval.keterangan);
            Assert.IsNotNull(approval.tanggalPengajuan);
        }

        [TestMethod]
        public void Test_UpdateApprovalStatus()
        {
            // Arrange
            var approval = new Approval("APV001", "B01", "Buku Test", "User Test");

            // Act - Perubahan status dari Pending ke Approved
            bool statusAwal = approval.status == "Pending";
            approval.status = "Approved";

            // Assert
            Assert.IsTrue(statusAwal);
            Assert.AreEqual("Approved", approval.status);
        }

        [TestMethod]
        public void Test_FilterApprovalByStatus()
        {
            // Arrange
            var approvals = new List<Approval>
            {
                new Approval("APV001", "B01", "Buku 1", "User 1"),
                new Approval("APV002", "B02", "Buku 2", "User 2") { status = "Approved" },
                new Approval("APV003", "B03", "Buku 3", "User 3") { status = "Rejected" },
                new Approval("APV004", "B04", "Buku 4", "User 4")
            };

            // Act
            var pendingApprovals = approvals.FindAll(a => a.status == "Pending");
            var approvedApprovals = approvals.FindAll(a => a.status == "Approved");
            var rejectedApprovals = approvals.FindAll(a => a.status == "Rejected");

            // Assert
            Assert.AreEqual(2, pendingApprovals.Count);
            Assert.AreEqual(1, approvedApprovals.Count);
            Assert.AreEqual(1, rejectedApprovals.Count);
        }
    }
}