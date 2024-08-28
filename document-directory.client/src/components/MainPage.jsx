import {useEffect, useState} from 'react';
import { Input, Space, Button, DatePicker, Radio, Table, Select, Popover, Modal } from 'antd';
import { FolderFilled, FolderAddFilled, FileAddFilled, FileFilled, CloseOutlined, DeleteFilled, LeftOutlined, EditFilled, LockFilled } from '@ant-design/icons';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import { useNavigate } from 'react-router-dom';
import Cookie from 'js-cookie';
import AccessManagePage from './AccessManagePage';
dayjs.extend(utc); 
const { Search } = Input;
const { TextArea } = Input;

function MainPageComponent() {
    const [availableNodes, setAvailableNodes] = useState([]);
    const [hierarchy, setHierarchy] = useState('/');
    const [isDirectoryCreateFormVisible, setIsDirectoryCreateFormVisible] = useState(false);
    const [isDocumentCreateFormVisible, setIsDocumentCreateFormVisible] = useState(false);
    const [isShowInfoChosenDirectory, setIsShowInfoChosenDirectory] = useState(false);
    const [isShowInfoChosenDocument, setIsShowInfoChosenDocument] = useState(false);
    const [isDocumentViewModalVisible, setIsDocumentViewModalVisible] = useState(false);
    const [isDocumentEditFormVisible, setIsDocumentEditFormVisible] = useState(false);
    const [isDirectoryEditFormVisible, setIsDirectoryEditFormVisible] = useState(false);
    const [chosenNode, setChosenNode] = useState({});
    const [viewMethod, setViewMethod] = useState('table');
    const [directoryHierarchy, setDirectoryHierarchy] = useState([]);
    const [dateFilterValue, setDateFilterValue] = useState(1);
    const [openDeleteModal, setOpenDeleteModal] = useState(false);
    const [confirmLoadingDeleteModal, setConfirmLoadingDeleteModal] = useState(false);
    const [isAccessManageModalVisible, setIsAccessManageModalVisible] = useState(false);
    const navigate = useNavigate();


    async function searchDocumentByName(value, _e, info) {
        setAvailableNodes([]);
        if (value != '') {
            const response = await fetch('https://localhost:7018/api/documents/filterBy', {
                method: 'POST',
                headers: new Headers({ "Content-Type": "application/json" }),
                credentials: 'include',
                body: JSON.stringify({
                    name: value
                })
            })
            if (response.status == 200) {
                const json = await response.json();
                json.map((document) => {
                    const activityEnd = dayjs(document.activityEnd, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    const createdAt = dayjs(document.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes((prevNodes) => [...prevNodes, {...document, activityEnd: activityEnd, createdAt: createdAt}]);
                })
            }
            
        } else {
            await getAvailableNodes();
        }
        
    } 

    async function createDirectory() {
        const name = document.querySelector('#inputDirectoryName').value;
        
        const response = await fetch('https://localhost:7018/api/folders', {
            method: 'POST', 
            headers: new Headers({ "Content-Type": "application/json" }), 
            credentials: 'include',
            body: JSON.stringify({
                name: name,
                folderId: directoryHierarchy.length !== 0 ? directoryHierarchy[directoryHierarchy.length - 1].id : 0
            })
        });
        if (response.status == 201)
        {
            const json = await response.json();
            const createdAt = dayjs(json.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY')
            setAvailableNodes([...availableNodes, {...json, createdAt: createdAt}]);
            setIsDirectoryCreateFormVisible(false);
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    async function createDocument() {
        const name = document.querySelector('#inputDocumentName').value;
        const inputActivityDate = document.querySelector('#inputDocumentActivityDate').value;
        const activityDate = dayjs.utc(inputActivityDate, 'DD-MM-YYYY').format();

        const content = document.querySelector('#inputDocumentContent').value;
        const response = await fetch('https://localhost:7018/api/documents', {
            method: 'POST', 
            headers: new Headers({ "Content-Type": "application/json" }),
            credentials: 'include',
            body: JSON.stringify({
                name: name,
                content: content,
                activityEnd: activityDate,
                folderId: directoryHierarchy.length !== 0 ? directoryHierarchy[directoryHierarchy.length - 1].id : 0
            })
        });
        if (response.status == 201)
        {
            const json = await response.json();
            const activityEnd = dayjs(json.activityEnd, 'YYYY-MM-DD').format('DD-MM-YYYY')
            const createdAt = dayjs(json.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY')
            setAvailableNodes([...availableNodes, {...json, activityEnd: activityEnd, createdAt: createdAt}]);
            setIsDocumentCreateFormVisible(false);
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    async function editDocument() {
        const name = document.querySelector('#inputEditDocumentName').value;
        const inputActivityDate = document.querySelector('#inputEditDocumentActivityDate').value;
        const activityDate = dayjs.utc(inputActivityDate, 'DD-MM-YYYY').format();
        const content = document.querySelector('#inputEditDocumentContent').value;
        const response = await fetch('https://localhost:7018/api/documents/' + chosenNode.id, {
            method: 'PATCH', 
            headers: new Headers({ "Content-Type": "application/json" }),
            credentials: 'include',
            body: JSON.stringify({
                name: name,
                content: content,
                activityEnd: activityDate,
            })
        });
        if (response.status == 200)
        {
            const json = await response.json();
            const activityEnd = dayjs(json.activityEnd, 'YYYY-MM-DD').format('DD-MM-YYYY');
            const createdAt = dayjs(json.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
            const editableDocument = {...json, activityEnd: activityEnd, createdAt: createdAt};
            setAvailableNodes(availableNodes.map((document) => document.id == editableDocument.id ? editableDocument : document));
            setIsDocumentEditFormVisible(false);
            setChosenNode(editableDocument);
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    async function editDirectory() {
        const name = document.querySelector('#inputEditDirectoryName').value;
        const response = await fetch('https://localhost:7018/api/folders/' + chosenNode.id, {
            method: 'PATCH', 
            headers: new Headers({ "Content-Type": "application/json" }), 
            credentials: 'include',
            body: JSON.stringify({
                name: name,
            })
        });
        if (response.status == 200)
        {
            const json = await response.json();
            const activityEnd = dayjs(json.activityEnd, 'YYYY-MM-DD').format('DD-MM-YYYY');
            const createdAt = dayjs(json.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
            const editableDirectory = {...json, activityEnd: activityEnd, createdAt: createdAt};
            setAvailableNodes(availableNodes.map((node) => node.id == editableDirectory.id ? editableDirectory : node));
            setIsDirectoryEditFormVisible(false);
            setChosenNode(editableDirectory);
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    async function deleteChosenNode() {
        let url = 'https://localhost:7018/api/'; 
        if (chosenNode.type == "Document") {
            url += 'documents';
        } else {
            url += 'folders';
        }
        url += '/' + chosenNode.id;
        const response = await fetch(url, {
            method: 'DELETE',
            headers: new Headers({"Content-Type": "application/json"}),
            credentials: 'include',
        });
        if (response.status == 200)
        {
            setAvailableNodes(availableNodes.filter((node) => node.id != chosenNode.id));
            setChosenNode({});
            setIsShowInfoChosenDirectory(false);
            setIsShowInfoChosenDocument(false);
            setOpenDeleteModal(false)
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    async function deleteNode(node) {
        setChosenNode(node);
        deleteChosenNode();
    }

    async function viewDirectoryContent(directory) {
        const response = await fetch(`https://localhost:7018/api/NodeHierarchy/${directory.id}`, {
            credentials: 'include',
        });
        if (response.status == 200)
        {
            setAvailableNodes([]);
            const json = await response.json();
            json.map((node) => {
                if (node.type == "Document")
                {
                    const activityEnd = dayjs(node.activityEnd, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    const createdAt = dayjs(node.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...node, activityEnd: activityEnd, createdAt: createdAt}]);
                }
                else {
                    const createdAt = dayjs(node.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...node, createdAt: createdAt}]);
                }
            });
            setIsShowInfoChosenDirectory(false);
            setIsShowInfoChosenDocument(false); 
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
        return response.status;
    }

    async function viewNextDirectoryContent() {
        let status = await viewDirectoryContent(chosenNode);
        if (status == 200) {
            if (directoryHierarchy.length != 0) {
                setHierarchy((hierarchy) => hierarchy + '/');
            }
            setHierarchy((hierarchy) => hierarchy + chosenNode.name);
            setDirectoryHierarchy([...directoryHierarchy, chosenNode]);
            setChosenNode({});
        }
    }

    async function viewPrevDirectoryContent() {
        if (directoryHierarchy.length != 0)
        {
            let prevDirectory = directoryHierarchy.flat().slice(directoryHierarchy.length - 2)[0];
            const indexPrevDirectory = hierarchy.lastIndexOf(directoryHierarchy[directoryHierarchy.length-1].name);
            setAvailableNodes([]);
            if (directoryHierarchy.length > 1)
            {
                let status = await viewDirectoryContent(prevDirectory);
                if (status == 200) {
                    console.log(hierarchy.substring(0, indexPrevDirectory));
                    setHierarchy((prevHierarchy) => prevHierarchy.substring(0, indexPrevDirectory));
                    if (hierarchy.length > 2) {
                        setHierarchy((prevHierarchy) => prevHierarchy.substring(0, prevHierarchy.lastIndexOf('/')));

                    }
                    setDirectoryHierarchy(directoryHierarchy.filter((directory) => directory != directoryHierarchy[directoryHierarchy.length - 1]));
                    setChosenNode({});
                }
            } else {
                await getAvailableNodes();
                setHierarchy((prevHierarchy) => prevHierarchy.substring(0, 1));
                
                setDirectoryHierarchy([]);
                setChosenNode({});
            }
            setIsShowInfoChosenDirectory(false);
            setIsShowInfoChosenDocument(false);
        }
        
    }

    async function editNode(node) {
        if (node != undefined)
        {
            setChosenNode(node);
            if (node.type == "Document") {
                setIsDocumentEditFormVisible(true);
            } else {
                setIsDirectoryEditFormVisible(true);
            }
        } else {
            if (chosenNode.type == "Document") {
                setIsDocumentEditFormVisible(true);
            } else {
                setIsDirectoryEditFormVisible(true);
            }
        }
    }

    async function getAvailableNodes() {
        const response = await fetch("https://localhost:7018/api/NodeHierarchy", {
            credentials: 'include',
        });
        if (response.status == 200)
        {
            setAvailableNodes([]);
            const json = await response.json();
            json.map((node) => {
                if (node.type == "Document")
                {
                    const activityEnd = dayjs(node.activityEnd, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    const createdAt = dayjs(node.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...node, activityEnd: activityEnd, createdAt: createdAt}]);
                }
                else {
                    const createdAt = dayjs(node.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...node, createdAt: createdAt}]);
                }
            });
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    async function filterByDate()
    {
        let startDate = document.querySelector("#filterStartDate").value;
        let endDate = document.querySelector("#filterEndDate").value;
        if (startDate != '' || endDate != '')
        {
            let filterBy = dateFilterValue == 1 ? "CreatedDate" : "ActivityDate";
            let startDateISO = '';
            let endDateISO = '';
            if (startDate != '') {
                startDateISO = dayjs.utc(startDate, 'DD-MM-YYYY').format();
            }
            if (endDate != '') {
                endDateISO = dayjs.utc(endDate, 'DD-MM-YYYY').format();
            }
            const response = await fetch("https://localhost:7018/api/documents/filterby", {
                method: 'POST',
                credentials: 'include',
                headers: new Headers({"Content-Type": "application/json"}),
                body: JSON.stringify({
                    startDate: startDateISO == '' ? null : startDateISO,
                    endDate: endDateISO == '' ? null : endDateISO,
                    filterBy: filterBy,
                })
            })
            if (response.status == 200) {
                setAvailableNodes([]);
                const json = await response.json();
                json.map((document) => {
                    const activityEnd = dayjs(document.activityEnd, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    const createdAt = dayjs(document.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...document, activityEnd: activityEnd, createdAt: createdAt}]);
                })
            }
            else if (response.status == 401) {
                navigate('/login');
                Cookie.remove('test')
            }
        }
    }

    const columns = [
        {
            title: 'Название',
            dataIndex: 'name',
            sorter: (a, b) => a.name.length - b.name.length,
            width: '40%',
        },
        {
            title: 'Тип',
            dataIndex: 'type',
            render: (type) => type == "Directory" ? "Директория" : "Документ",
            filters: [
                {
                    text: 'Документ',
                    value: 'Document',
                },
                {
                    text: 'Директория',
                    value: 'Directory',
                },
            ],
            width: '8%',
        },
        {
            title: 'Дата создания',
            dataIndex: 'createdAt',
            width: '12%'
        },
        {
            title: 'Дата активности',
            dataIndex: 'activityEnd',
            width: '10%'
        },
        {
            width: '10%',
            render: (_, record) => <a onClick={() => editNode(record)}>Редактировать</a>
        },
        {
            width: '10%',
            render: (_, record) => <a onClick={() => {setChosenNode(record); setOpenDeleteModal(true)}}>Удалить</a>
        },
        {
            width: '10%',
            render: (_, record) => <a onClick={() => {setChosenNode(record); setIsAccessManageModalVisible(true)}}>Управление доступом</a>
        }
    ];

    useEffect(() => {
        getAvailableNodes();
    }, [])

    return (
        <>
            {isDirectoryCreateFormVisible && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div style={{display: 'flex', justifyContent: 'space-between'}}>
                            <Input placeholder='Введите название папки' id='inputDirectoryName' style={{width: '70%'}}/>
                            <CloseOutlined onClick={() => setIsDirectoryCreateFormVisible(false)}/>
                        </div>
                        <Button size='small' onClick={createDirectory} style={{width: "100px"}}>Сохранить</Button>
                    </div>
                </div>
            )}
            {isDirectoryEditFormVisible && Object.keys(chosenNode).length !== 0 && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div style={{display: 'flex', justifyContent: 'space-between'}}>
                            <Input placeholder='Введите название папки' id='inputEditDirectoryName' style={{width: '70%'}} defaultValue={chosenNode.name}/>
                            <CloseOutlined onClick={() => setIsDirectoryEditFormVisible(false)}/>
                        </div>
                        <Button size='small' onClick={editDirectory} style={{width: "100px"}}>Сохранить</Button>
                    </div>
                </div>
            )}
            {isDocumentCreateFormVisible && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div style={{display: 'flex', justifyContent: 'space-between'}}>
                            <Input placeholder='Введите название документа' id='inputDocumentName' style={{width: '70%'}}/>
                            <CloseOutlined onClick={() => setIsDocumentCreateFormVisible(false)}/>
                        </div>
                        <div style={{display: 'flex', gap: '10px', alignItems: 'center'}}>
                            <p>Дата окончания действия документа</p>
                            <DatePicker id="inputDocumentActivityDate" format={{format: 'DD-MM-YYYY'}} placeholder='Выберите дату' placement='bottomLeft' minDate={dayjs()}/>
                        </div>
                        <TextArea
                            placeholder="Введите содержимое документа"
                            autoSize={{
                                minRows: 6,
                                maxRows: 10
                            }}
                            id="inputDocumentContent"
                        />
                        <Button size='small' onClick={createDocument} style={{width: "100px"}}>Сохранить</Button>
                    </div>
                </div>
            )}
            {isDocumentEditFormVisible && Object.keys(chosenNode).length !== 0 && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div style={{display: 'flex', justifyContent: 'space-between'}}>
                            <Input placeholder='Название документа' id='inputEditDocumentName' style={{width: '70%'}} defaultValue={chosenNode.name}/>
                            <CloseOutlined onClick={() => setIsDocumentEditFormVisible(false)}/>
                        </div>
                        <div style={{display: 'flex', gap: '10px', alignItems: 'center', justifyContent: 'space-between'}}>
                            <div style={{display: 'flex', gap: '10px'}}>
                                <p>Дата окончания действия документа</p>
                                <DatePicker id="inputEditDocumentActivityDate" format={{format: 'DD-MM-YYYY'}} placeholder='Выберите дату' placement='bottomLeft' minDate={dayjs.utc()} defaultValue={dayjs(chosenNode.activityEnd, 'DD-MM-YYYY')}/>
                            </div>
                            {chosenNode.type == "Document" && dayjs.utc(chosenNode.activityEnd, 'DD-MM-YYYY') < dayjs.utc() && <p style={{color: 'red'}}>Срок действия документа истек</p>}
                        </div>
                        <TextArea
                            placeholder="Введите содержимое документа"
                            autoSize={{
                                minRows: 6,
                                maxRows: 10
                            }}
                            id="inputEditDocumentContent"
                            defaultValue={chosenNode.content}
                        />
                        <Button size='small' onClick={editDocument} style={{width: "100px"}}>Сохранить</Button>
                    </div>
                </div>
            )}
            {isDocumentViewModalVisible && Object.keys(chosenNode).length !== 0 && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div style={{display: 'flex', justifyContent: 'space-between', alignItems: 'center'}}>
                            <div>
                                <h2 style={{margin: '0', textWrap: 'wrap', maxWidth: '100%'}}>{chosenNode.name}</h2>
                            </div>
                            <CloseOutlined onClick={() => setIsDocumentViewModalVisible(false)}/>
                        </div>
                        <div style={{display: 'flex', gap: '10px', alignItems: 'center'}}>
                            <p>Дата создания документа</p>
                            <DatePicker disabled format={{format: 'DD-MM-YYYY'}} placement='bottomLeft' defaultValue={dayjs(chosenNode.createdAt, "DD-MM-YYYY")} style={{color: 'black'}}/>
                        </div>
                        <div style={{display: 'flex', gap: '10px', alignItems: 'center', justifyContent: 'space-between'}}>
                            <div style={{display: 'flex', gap: '10px'}}>
                                <p>Дата окончания действия документа</p>
                                <DatePicker disabled format={{format: 'DD-MM-YYYY'}} placement='bottomLeft' defaultValue={dayjs(chosenNode.activityEnd, "DD-MM-YYYY")}/>
                            </div>
                            {chosenNode.type == "Document" && dayjs.utc(chosenNode.activityEnd, 'DD-MM-YYYY') < dayjs.utc() && <p style={{color: 'red'}}>Срок действия документа истек</p>}
                        </div>
                        <TextArea disabled defaultValue={chosenNode.content} style={{textWrap: 'wrap', textAlign: 'left', opacity: '1', color: 'black', cursor: 'pointer'}} autoSize={{minRows: 18, maxRows: 20}}/>
                    </div>
                </div>
            )}
            {isAccessManageModalVisible && Object.keys(chosenNode).length !== 0 && <AccessManagePage 
                setIsModalVisible={setIsAccessManageModalVisible} 
                chosenNode={chosenNode}
                setChosenNode={setChosenNode}
            />}
            {chosenNode && Object.keys(chosenNode).length !== 0 && <Modal 
                title={"Подтверждение удаления"}
                open={openDeleteModal}
                onOk={deleteChosenNode}
                confirmLoading={confirmLoadingDeleteModal}
                onCancel={() => setOpenDeleteModal(false)}
                okText="Принять"
                cancelText="Отменить"
            >
                <p>{`Вы действительно хотите удалить ${chosenNode.type == "Document" ? "документ" : "папку"} "${chosenNode.name}"`}</p>
            </Modal>}
            <div>
                <Search
                    placeholder="Введите название документа"
                    allowClear
                    onSearch={searchDocumentByName}
                    style={{
                        maxWidth: '700px',
                        width: '70%'
                    }}
                />
                <div style={{display: 'flex', gap: '5px', marginTop: '15px', alignItems: 'center'}}>
                    <div style={{display: 'flex', gap: '10px', alignItems: 'center'}}>
                        <p style={{ margin: '0', textAlign: 'left'}}>Фильтрация документов по дате</p>
                        <Radio.Group style={{display: 'flex'}} defaultValue={dateFilterValue}>
                            <Radio value={1} onClick={() => setDateFilterValue(1)}>создания</Radio>
                            <Radio value={2} onClick={() => setDateFilterValue(2)}>активности</Radio>
                        </Radio.Group>
                    </div>
                    <div style={{display: 'flex', gap: '5px'}}>
                        <div style={{display: 'flex', gap: '10px', alignItems: 'center'}}>
                            <p>с</p>
                            <DatePicker placeholder="Укажите дату" style={{width: '130px'}} id="filterStartDate" format={{format: 'DD-MM-YYYY'}}/>
                        </div>
                        <div style={{display: 'flex', gap: '10px', alignItems: 'center'}}>
                            <p>до</p>
                            <DatePicker placeholder="Укажите дату" style={{width: '130px'}} id="filterEndDate" format={{format: 'DD-MM-YYYY'}}/>
                        </div>
                    </div>
                    <Button onClick={filterByDate}>Применить</Button>
                    <CloseOutlined onClick={getAvailableNodes}/>
                </div>
            </div>
            <div style={{display: 'flex', justifyContent: 'space-between', marginTop: '5px'}}>
                <div style={{display: 'flex', gap: '15px'}}>
                    <Popover content={<p style={{fontSize: '14px'}}>Создать папку</p>}>
                        <button onClick={() => setIsDirectoryCreateFormVisible(true)} className="btnWithIcon">
                            <FolderAddFilled style={{fontSize: '30px'}}/>
                        </button>
                    </Popover>
                    <Popover content={<p style={{fontSize: '14px'}}>Создать документ</p>}>
                        <button onClick={() => setIsDocumentCreateFormVisible(true)} className="btnWithIcon">
                            <FileAddFilled style={{fontSize: '30px'}}/>
                        </button>
                    </Popover>
                    <Popover content={<p style={{fontSize: '14px'}}>Удалить выбранный элемент</p>}>
                        <button onClick={() => {Object.keys(chosenNode).length !== 0 && setOpenDeleteModal(true)}} className="btnWithIcon">
                            <DeleteFilled style={{fontSize: '30px'}}/>
                        </button>
                    </Popover>
                    <Popover content={<p style={{fontSize: '14px'}}>Редактировать</p>}>
                        <button onClick={() => {Object.keys(chosenNode).length !== 0 && editNode()}} className="btnWithIcon">
                            <EditFilled style={{fontSize: '30px'}}/>
                        </button>
                    </Popover>
                    <Popover content={<p style={{fontSize: '14px'}}>Управление доступом</p>}>
                        <button onClick={() => {Object.keys(chosenNode).length !== 0 && setIsAccessManageModalVisible(true)}} className="btnWithIcon">
                            <LockFilled style={{fontSize: '30px'}}/>
                        </button>
                    </Popover>
                </div>
                {isShowInfoChosenDirectory && !isShowInfoChosenDocument && <div style={{fontSize: '14px', maxSize: '40%', height: '30px'}}>
                    <p>Имя папки: {chosenNode.name}</p>
                    <p>Дата создания: {chosenNode.createdAt}</p>
                </div>}
                {isShowInfoChosenDocument && !isShowInfoChosenDirectory && <div style={{fontSize: '14px', maxSize: '40%', height: '30px'}}>
                    <p>Имя документа: {chosenNode.name}</p>
                    <p>Дата создания: {chosenNode.createdAt}</p>
                    <p>Дата активности: {chosenNode.activityEnd}</p>
                </div>}
            </div>
            <div style={{display: 'flex', gap: '10px'}}>
                <button onClick={() => viewPrevDirectoryContent()} className="btnWithIcon">
                    <LeftOutlined style={{fontSize: '15px'}}/>
                </button>
                <p style={{textAlign: 'left', fontSize: '18px'}}>{hierarchy}</p>
            </div>
            <div style={{display: 'flex', alignItems: 'center', gap: '20px', marginBottom: '10px'}}>
                <p>Способ отображения</p>
                <Select onChange={setViewMethod} style={{width: '100px'}} defaultValue={'table'} options={[
                    { value: 'table', label: 'Таблица'},
                    { value: 'icons', label: 'Значки'}
                ]}/>
            </div>
            {viewMethod == 'icons' && <div>
                <Space wrap style={{gap: '15px'}}>
                    {availableNodes.map((node, index) => (
                        <div key={index} className='availableNode' style={{maxWidth: '120px'}}>
                            {node.type == "Directory" ? 
                            <button className="btnWithIcon" onClick={() => {setIsShowInfoChosenDocument(false); setChosenNode(node); setIsShowInfoChosenDirectory(true);}} onDoubleClick={() => viewNextDirectoryContent()}>
                                <FolderFilled style={{fontSize: '50px'}} />
                                <p style={{width: '110px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap'}}>{node.name}</p>
                            </button>
                            :
                            <button className="btnWithIcon" onClick={() => {setIsShowInfoChosenDirectory(false); setChosenNode(node); setIsShowInfoChosenDocument(true);}} onDoubleClick={() => setIsDocumentViewModalVisible(true)}>
                                <FileFilled style={{fontSize: '45px'}}/>
                                <p style={{width: '110px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap'}}>{node.name}</p> 
                            </button>
                            }
                        </div>
                    ))}
                </Space>
            </div>}
            {viewMethod == 'table' && <Table dataSource={availableNodes} columns={columns} rowKey={(node) => node.id}/>}
        </>
    );
}

export default MainPageComponent;