import 'vuetify/styles'
import '@mdi/font/css/materialdesignicons.css'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

const vuetify = createVuetify({
  components,
  directives,
  theme: {
    defaultTheme: 'light',
    themes: {
      light: {
        colors: {
          primary: '#5C6BC0',
          secondary: '#26A69A',
          accent: '#FF7043',
          error: '#EF5350',
          warning: '#FFA726',
          info: '#42A5F5',
          success: '#66BB6A',
          background: '#F5F5F5',
          surface: '#FFFFFF',
        },
      },
      dark: {
        colors: {
          primary: '#7986CB',
          secondary: '#4DB6AC',
          accent: '#FF8A65',
          error: '#EF5350',
          warning: '#FFA726',
          info: '#42A5F5',
          success: '#66BB6A',
          background: '#121212',
          surface: '#1E1E1E',
        },
      },
    },
  },
})

export default vuetify
