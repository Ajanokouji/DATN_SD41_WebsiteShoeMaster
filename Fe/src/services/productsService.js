import axios from 'axios';

const API_BASE_URL = 'YOUR_API_BASE_URL'; // Thay thế bằng URL gốc của API

export const getProducts = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/products`);
    return response.data;
  } catch (error) {
    console.error('Error fetching products:', error);
    throw error;
  }
};

export const getProductById = async (id) => {
  try {
    const response = await axios.get(`${API_BASE_URL}/products/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching product with id ${id}:`, error);
    throw error;
  }
};

export const createProduct = async (product) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/products`, product);
    return response.data;
  } catch (error) {
    console.error('Error creating product:', error);
    throw error;
  }
};

export const updateProduct = async (id, product) => {
  try {
    const response = await axios.put(`${API_BASE_URL}/products/${id}`, product);
    return response.data;
  } catch (error) {
    console.error(`Error updating product with id ${id}:`, error);
    throw error;
  }
};

export const deleteProduct = async (id) => {
  try {
    const response = await axios.delete(`${API_BASE_URL}/products/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error deleting product with id ${id}:`, error);
    throw error;
  }
};

export const filterProducts = async (queryParams) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/products/filter`, null, { params: queryParams });
    return response.data;
  } catch (error) {
    console.error('Error filtering products:', error);
    throw error;
  }
};

export const countProducts = async (queryParams) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/products/count`, null, { params: queryParams });
    return response.data;
  } catch (error) {
    console.error('Error counting products:', error);
    throw error;
  }
};

export const getProductCategoriesRelationById = async (id) => {
  try {
    const response = await axios.get(`${API_BASE_URL}/ProductCategoriesRelation/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching product categories relation with id ${id}:`, error);
    throw error;
  }
};

export const updateProductCategoriesRelation = async (id, relation) => {
  try {
    const response = await axios.patch(`${API_BASE_URL}/ProductCategoriesRelation/${id}`, relation);
    return response.data;
  } catch (error) {
    console.error(`Error updating product categories relation with id ${id}:`, error);
    throw error;
  }
};

export const deleteProductCategoriesRelation = async (id) => {
  try {
    const response = await axios.delete(`${API_BASE_URL}/ProductCategoriesRelation/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error deleting product categories relation with id ${id}:`, error);
    throw error;
  }
};

export const filterProductCategoriesRelations = async (queryParams) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/ProductCategoriesRelation/filter`, null, { params: queryParams });
    return response.data;
  } catch (error) {
    console.error('Error filtering product categories relations:', error);
    throw error;
  }
};

export const countProductCategoriesRelations = async (queryParams) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/ProductCategoriesRelation/count`, null, { params: queryParams });
    return response.data;
  } catch (error) {
    console.error('Error counting product categories relations:', error);
    throw error;
  }
};

export const createProductCategoriesRelation = async (relation) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/ProductCategoriesRelation`, relation);
    return response.data;
  } catch (error) {
    console.error('Error creating product categories relation:', error);
    throw error;
  }
};

export const deleteProductCategoriesRelations = async (ids) => {
  try {
    const response = await axios.delete(`${API_BASE_URL}/ProductCategoriesRelation`, { data: ids });
    return response.data;
  } catch (error) {
    console.error('Error deleting product categories relations:', error);
    throw error;
  }
};