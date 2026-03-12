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
          primary:    '#6b9080',  // Jungle Teal
          secondary:  '#a4c3b2',  // Muted Teal
          accent:     '#cce3de',  // Frozen Water
          background: '#eaf4f4',  // Azure Mist
          surface:    '#f6fff8',  // Mint Cream
          error:      '#c0544a',
          warning:    '#c08b3a',
          info:       '#4a8fa4',
          success:    '#5a9070',
          'on-primary':    '#ffffff',
          'on-secondary':  '#263d35',
          'on-background': '#263d35',
          'on-surface':    '#263d35',
        },
      },
      dark: {
        colors: {
          primary:    '#6b9080',  // Jungle Teal
          secondary:  '#a4c3b2',  // Muted Teal
          accent:     '#cce3de',  // Frozen Water
          background: '#0f1e1a',
          surface:    '#1a2e27',
          error:      '#e07068',
          warning:    '#e0a855',
          info:       '#6ab4cc',
          success:    '#7ab898',
          'on-primary':    '#ffffff',
          'on-secondary':  '#0f1e1a',
          'on-background': '#dff0e8',
          'on-surface':    '#dff0e8',
        },
      },
    },
  },
})

export default vuetify
