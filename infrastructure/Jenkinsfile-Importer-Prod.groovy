pipeline {
    agent any

    environment {
	      ASPNETCORE_ENVIRONMENT = "Production"
              DOCKER_PROJECT_NAME = "odh-tourism-importer"
              DOCKER_IMAGE = '755952719952.dkr.ecr.eu-west-1.amazonaws.com/odh-tourism-importer'
              DOCKER_TAG = "test-$BUILD_NUMBER"
	      SERVER_PORT = "1029"        
              PG_CONNECTION = credentials('odh-tourism-api-prod-pg-connection')
	      MSS_USER = credentials('odh-tourism-api-test-mss-user')
	      MSS_PSWD = credentials('odh-tourism-api-test-mss-pswd')
	      LCS_USER = credentials('odh-tourism-api-test-lcs-user')
	      LCS_PSWD = credentials('odh-tourism-api-test-lcs-pswd')
	      LCS_MSGPSWD = credentials('odh-tourism-api-test-lcs-msgpswd')
	      SIAG_USER = credentials('odh-tourism-api-test-siag-user')
	      SIAG_PSWD = credentials('odh-tourism-api-test-siag-pswd')
	      XMLDIR = credentials('odh-tourism-api-test-xmldir')
	      IMG_URL = "https://images.tourism.testingmachine.eu/api/Image/GetImage?imageurl="
	      DOC_URL = "https://images.tourism.testingmachine.eu/api/File/GetFile/"
	      S3_BUCKET_ACCESSPOINT = credentials('odh-tourism-api-test-bucket-accesspoint')
	      S3_IMAGEUPLOADER_ACCESSKEY = credentials('odh-tourism-api-test-s3-imageuploader-accesskey')
	      S3_IMAGEUPLOADER_SECRETKEY = credentials('odh-tourism-api-test-s3-imageuploader-secretkey')
	      OAUTH_AUTORITY = "https://auth.opendatahub.testingmachine.eu/auth/realms/noi/"
	      ELK_URL = credentials('odh-tourism-api-test-elk-url')
	      ELK_TOKEN = credentials('odh-tourism-api-test-elk-token')
	      JSONPATH = "./wwwroot/json/"
	      EBMS_USER = credentials('odh-tourism-api-test-ebms-user')
	      EBMS_PASS = credentials('odh-tourism-api-test-ebms-pass')
	      DATABROWSER_URL = "https://frontend.tourism.testingmachine.eu/"
	      RAVEN_SERVICEURL = "https://service.suedtirol.info/api/"
	      RAVEN_USER = credentials('odh-raven-api-user')
	      RAVEN_PSWD = credentials('odh-raven-api-pswd')
	      API_URL = "https://tourism.api.opendatahub.bz.it/v1/"
    }

    stages {
        stage('Configure') {
            steps {
                sh """
                    rm -f .env
                    cp .env.example .env
                    echo 'COMPOSE_PROJECT_NAME=${DOCKER_PROJECT_NAME}' > .env
                    echo 'DOCKER_IMAGE=${DOCKER_IMAGE}' >> .env
                    echo 'DOCKER_TAG=${DOCKER_TAG}' >> .env
                    echo 'SERVER_PORT=${SERVER_PORT}' >> .env         
		                echo 'ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}' >> .env         
                    echo 'PG_CONNECTION=${PG_CONNECTION}' >> .env
		                echo 'MSS_USER=${MSS_USER}' >> .env
		                echo 'MSS_PSWD=${MSS_PSWD}' >> .env
		                echo 'LCS_USER=${LCS_USER}' >> .env
		                echo 'LCS_PSWD=${LCS_PSWD}' >> .env
		                echo 'LCS_MSGPSWD=${LCS_MSGPSWD}' >> .env
		                echo 'SIAG_USER=${SIAG_USER}' >> .env
		                echo 'SIAG_PSWD=${SIAG_PSWD}' >> .env
		                echo 'XMLDIR=${XMLDIR}' >> .env
		                echo 'IMG_URL=${IMG_URL}' >> .env
		                echo 'DOC_URL=${DOC_URL}' >> .env
		                echo 'S3_BUCKET_ACCESSPOINT=${S3_BUCKET_ACCESSPOINT}' >> .env
		                echo 'S3_IMAGEUPLOADER_ACCESSKEY=${S3_IMAGEUPLOADER_ACCESSKEY}' >> .env
		                echo 'S3_IMAGEUPLOADER_SECRETKEY=${S3_IMAGEUPLOADER_SECRETKEY}' >> .env
		                echo 'OAUTH_AUTORITY=${OAUTH_AUTORITY}' >> .env
		                echo 'ELK_URL=${ELK_URL}' >> .env
		                echo 'ELK_TOKEN=${ELK_TOKEN}' >> .env
		                echo 'JSONPATH=${JSONPATH}' >> .env
		                echo 'EBMS_USER=${EBMS_USER}' >> .env
		                echo 'EBMS_PASS=${EBMS_PASS}' >> .env
		                echo 'DATABROWSER_URL=${DATABROWSER_URL}' >> .env		
		                echo 'RAVEN_SERVICEURL=${RAVEN_SERVICEURL}' >> .env
		                echo 'RAVEN_USER=${RAVEN_USER}' >> .env
		                echo 'RAVEN_PSWD=${RAVEN_PSWD}' >> .env
		                echo 'API_URL=${API_URL}' >> .env
                """
            }
        }
        stage('Build') {
            steps {
                sh '''
                    aws ecr get-login --region eu-west-1 --no-include-email | bash
                    docker-compose --no-ansi -f OdhApiImporter/docker-compose.yml build --pull
                    docker-compose --no-ansi -f OdhApiImporter/docker-compose.yml push
                '''
            }
        }
        stage('Deploy') {
            steps {
               sshagent(['jenkins-ssh-key']) {
                    sh """
                        (cd infrastructure/ansible && ansible-galaxy install -f -r requirements.yml)
                        (cd infrastructure/ansible && ansible-playbook --limit=prod deploy-importer.yml --extra-vars "release_name=${BUILD_NUMBER}")
                    """
                }
            }
        }
    }
}
