/**
 * @param { import("knex").Knex } knex
 * @returns { Promise<void> } 
 */
exports.seed = async function(knex) {
  // Deletes ALL existing entries
  await knex('nhanvien').del();

  // Inserts seed entries
  await knex('nhanvien').insert([
    { tennhanvien: 'Trần Cao Thị Thủy Tiên', chucvu: 'Phục vụ', luong: 5000000, ngayvaolam: '2024-01-15', trangthai: 1,
      username: knex.raw("decode('ApINhQ/AnVGux+9KksWiTQ==', 'base64')"), 
      iv_username: knex.raw("decode('hxgfNbYiSwQSdHvkU/v81g==', 'base64')"),
      password: knex.raw("decode('6IIut12Hw6HW0GZgVUxp/Q==', 'base64')"),
      iv_password: knex.raw("decode('OhDxr/LMAtSCwPuAq0f3xA==', 'base64')") 
     },
    { tennhanvien: 'Lê Ngọc Trâm', chucvu: 'Thu ngân', luong: 6000000, ngayvaolam: '2024-02-15', trangthai: 1 },
    { tennhanvien: 'Nguyễn Thị Cẩm Tú', chucvu: 'Phục vụ', luong: 5000000, ngayvaolam: '2024-03-15', trangthai: 1 },
    { tennhanvien: 'Nguyễn Minh Thư', chucvu: 'Đầu bếp', luong: 6000000, ngayvaolam: '2024-04-01', trangthai: 0 },
    { tennhanvien: 'Lê Thảo Uyên', chucvu: 'Đầu bếp', luong: 6000000, ngayvaolam: '2024-01-07', trangthai: 1 },
    { tennhanvien: 'Nguyễn Thạch Thảo', chucvu: 'Phục vụ', luong: 5000000, ngayvaolam: '2024-02-10', trangthai: 1 },
    { tennhanvien: 'Dương Hồng Tuyết', chucvu: 'Phục vụ', luong: 5000000, ngayvaolam: '2024-03-15', trangthai: 1 },
    { tennhanvien: 'Nguyễn Tuyết Ngân', chucvu: 'Phục vụ', luong: 5000000, ngayvaolam: '2024-02-19', trangthai: 0 },
    { tennhanvien: 'Nguyễn Thu Trang', chucvu: 'Quản lý', luong: 15000000, ngayvaolam: '2024-02-19', trangthai: 1, 
      username: knex.raw("decode('Mhustbozjp8+pd5xoFLpQQ==', 'base64')"), 
      iv_username: knex.raw("decode('gwAXVYwGRGpvFxL0PlAoow==', 'base64')"),
       password: knex.raw("decode('kauSlUNzFutJW8T6hVKObQ==', 'base64')"),
        iv_password: knex.raw("decode('mMGh/QGWPP/X+5JlBUBuOg==', 'base64')") },
  ]);
};
