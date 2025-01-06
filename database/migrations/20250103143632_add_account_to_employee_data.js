/**
 * @param { import("knex").Knex } knex
 * @returns { Promise<void> }
 */
exports.up = async function(knex) {
    await knex.raw(`
    ALTER TABLE nhanvien
    add column username bytea default null,
    add column password bytea default null,
    add column iv_username bytea default null,
    add column iv_password bytea default null;
    `);
};

/**
 * @param { import("knex").Knex } knex
 * @returns { Promise<void> }
 */
exports.down = async function(knex) {
    await knex.raw(`
    ALTER TABLE nhanvien
    drop column username,
    drop column password,
    drop column iv_username,
    drop column iv_password;
    `);
};

