pipeline {
    agent any

    environment {
        APPSETTINGS = credentials('istapio.appsettings.production.json')
    }

    stages {
        stage('Prepare') {
            steps {
                sh '''
                    cp "$APPSETTINGS" "$WORKSPACE/Presentation/Istapio.API/appsettings.json"
                    chmod 644 "$WORKSPACE/Presentation/Istapio.API/appsettings.json"
                '''
            }
        }

        stage('Deploy') {
            steps {
                sh 'docker compose up -d --build'
            }
        }
    }
}