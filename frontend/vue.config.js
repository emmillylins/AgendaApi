module.exports = {
  devServer: {
    https: false, // Desativa HTTPS local
    proxy: {
      '/api': {
        target: 'https://localhost:44380',
        changeOrigin: true,
        secure: false, // Ignora erros de certificado SSL
        ws: true
      }
    }
  }
}